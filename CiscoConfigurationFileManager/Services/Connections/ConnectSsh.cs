using CiscoConfigurationFileManager.Services.Configurations;
using CiscoConfigurationFileManager.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CiscoConfigurationFileManager.Services.Connections;

/// <summary>
/// Implements a SSH connection to a Cisco device.
/// </summary>
public class ConnectSsh : Connection
{
    private readonly IMessenger _messenger = WeakReferenceMessenger.Default;
    private readonly SshClient _ssh;
    private readonly TftpConnection _tftpServer;
    private readonly TaskCompletionSource<List<string>> _downloadConfig = new();
    private readonly TaskCompletionSource _uploadConfig = new();
    private ShellStream? _stream;
    private bool _isConfigMerged;

    /// Initializes a new instance of the ConnectSsh class.
    /// </summary>
    /// <param name="ip">The IP address of the device to connect to.</param>
    /// <param name="username">The username to use for authentication.</param>
    /// <param name="password">The password to use for authentication.</param>
    /// <param name="secret">The enable secret to use for authentication.</param>
    public ConnectSsh(string? ip, string? username, string? password, string? secret)
    {
        var connectionInfo = new ConnectionInfo(ip, 22, username, new PasswordAuthenticationMethod(username, password));
        _ssh = new SshClient(connectionInfo);
        _tftpServer = TftpConnection.Instance;
        Secret = secret;
    }

    /// <inheritdoc/>
    public override void Connect()
    {
        _ssh.Connect();
        _stream = _ssh.CreateShellStream(string.Empty, 0, 0, 0, 0, 0);
        _stream.DataReceived += SshDataReceived;
    }

    /// <inheritdoc/>
    public override void SendCommand(string? command)
    {
        _stream?.WriteLine(command);
    }

    /// <inheritdoc/>
    public override void SendModule(List<string> module)
    {
        foreach (var line in module)
        {
            Thread.Sleep(500);
            SendCommand(line);
        }
    }

    /// <inheritdoc/>
    public override async Task<List<string>> RetrieveConfig()
    {
        await AwaitConfMode();

        IfFileExistsDelete(Paths.TftpRunningConfig);

        var commands = ReplaceIp((await File.ReadAllLinesAsync(Paths.TftpDownloadCommands)).ToList());

        SendModule(commands);

        var fw = new FileSystemWatcher(_tftpServer.ServerDirectory!);
        fw.Created += new FileSystemEventHandler(FileCreated);
        fw.EnableRaisingEvents = true;

        return await _downloadConfig.Task;
    }

    /// <inheritdoc/>
    public override async void UploadConfig(List<string> config)
    {
        _isConfigMerged = false;
        CheckConfMode = true;
        SendCommand("");
        await AwaitConfMode();

        var file = Paths.TftpUploadCommands;
        var conf = Paths.TftpConfigToUpload;
        if (!File.Exists(conf))
        {
            File.Create(conf).Close();
        }

        await File.WriteAllLinesAsync(conf, config);

        var commands = ReplaceIp((await File.ReadAllLinesAsync(file)).ToList());


        SendModule(commands);

        MessageBoxProvider.ShowOk("Upload of configuration started", "Upload", "Ok", () => { });


        var uploadTask = AwaitConfigMerge();

        if (await Task.WhenAny(uploadTask, Task.Delay(15000)) != uploadTask)
        {
            throw new TimeoutException("Upload timed out.");
        }

        CopyToStartupConfig();
    }

    /// <summary>
    /// Replaces IP addresses in a list of commands with the local IP address.
    /// </summary>
    /// <param name="commands">The list of commands to replace IP addresses in.</param>
    /// <returns>A list of commands with the IP addresses replaced.</returns>
    public List<string?> ReplaceIp(List<string> commands)
    {
        var ip = Dns
            .GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            ?.ToString() ?? string.Empty;
        return new List<string?>(ParameterReplacer.ReplaceParameter(commands,
            "${ip}$", ip));
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        _ssh.Dispose();
        _stream?.Dispose();
        _tftpServer.Dispose();
    }

    /// <summary>
    /// Called when a file is created. Reads the lines of the file and sets the download configuration result.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The arguments for the event.</param>
    private void FileCreated(object sender, FileSystemEventArgs e)
    {
        var fileInfo = new FileInfo(e.FullPath);
        var timer = new System.Timers.Timer(15000);
        timer.Elapsed += (source, eventArgs) =>
        {
            timer.Stop();
            timer.Dispose();
            if (!IsFileLocked(fileInfo)) return;
            throw new InvalidOperationException($"The file {e.FullPath} is still locked");
        };
        timer.Start();

        while (IsFileLocked(fileInfo))
        {
            Thread.Sleep(100);
        }

        var lines = new List<string>(File.ReadAllLines(e.FullPath));
        _downloadConfig.SetResult(lines);
    }

    /// <summary>
    /// Returns a task that completes when the configuration has been merged.
    /// </summary>
    /// <returns>A task that completes when the configuration has been merged.</returns>
    private async Task<Task> AwaitConfigMerge()
    {
        //while (!_isConfigMerged)
        //{
        //    await Task.Delay(100);
        //}

        return _uploadConfig.Task;
    }

    /// Determines whether a file is currently locked.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns>True if the file is locked; otherwise, false.</returns>
    private static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            stream.Close();
        }
        catch (IOException)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Deletes a file if it exists and is not locked.
    /// </summary>
    /// <param name="path">The path of the file to delete.</param>
    private static void IfFileExistsDelete(string path)
    {
        if (!File.Exists(path)) return;
        IsFileLocked(new FileInfo(path));
        File.Delete(path);
    }

    /// <summary>
    /// Called when data is received from an SSH shell. Checks for certain keywords and sets flags accordingly.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The arguments for the event.</param>
    private void SshDataReceived(object? sender, ShellDataEventArgs e)
    {
        if (e.Data is not {Length: > 1}) return;
        var str = Encoding.Default.GetString(e.Data);
        if (str == "\r\n") return;
        if (str.Contains("?"))
        {
            SendCommand("");
        }

        if (!_isConfigMerged)
        {
            if (str.Contains("OK"))
            {
                _isConfigMerged = true;

                _uploadConfig.SetResult();
                Application.Current.Dispatcher.BeginInvoke(
                    new Action(ModuleViewModel.ShowChangesDetectedDialog),
                    DispatcherPriority.Background);
            }
        }

        if (CheckConfMode)
            SetConfigurationMode(str);

        WriteData(str);
    }

    private void WriteData(string text)
    {
    }
}