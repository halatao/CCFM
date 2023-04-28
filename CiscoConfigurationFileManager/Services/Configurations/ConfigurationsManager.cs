using CiscoConfigurationFileManager.Models;
using CiscoConfigurationFileManager.Services.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CiscoConfigurationFileManager.Services.Configurations;

public class ConfigurationsManager
{
    private Connection? _connection;
    public Configuration? Configuration { get; private set; }

    public void SetupSerial(string? comPort)
    {
        _connection = new ConnectSerial(comPort, 9600);
        _connection?.Connect();
    }

    public void SetupSsh(string? ip, string? username, string? password, string? secret)
    {
        _connection = new ConnectSsh(ip, username, password, secret);
        _connection?.Connect();
    }

    public void SetupFlatFile()
    {
        _connection = new ConnectFlatFile();
    }

    public async Task DownloadConfig()
    {
        var downloadTask = _connection?.RetrieveConfig();

        if (await Task.WhenAny(downloadTask, Task.Delay(15000)) != downloadTask)
        {
            throw new TimeoutException("Download timed out.");
        }

        var config = await downloadTask!;
        Configuration = new Configuration(config);
    }


    public void SendCommand(string? command)
    {
        _connection?.SendCommand(command);
    }

    public void UploadConfig()
    {
        _connection?.UploadConfig(Configuration?.ConfigurationToUpload ?? new List<string>());
    }

    public void UpdateConfig(List<RunningModule>? modules)
    {
        Configuration?.UpdateModules(modules);
    }

    public void SaveAsConfig(string path)
    {
        File.WriteAllLines(path, Configuration?.ConfigurationToUpload ?? new List<string>());
    }

    public void OpenConfig(string path)
    {
        Configuration?.OpenConfiguration(path);
    }

    public void NewConfig()
    {
        Configuration?.NewConfiguration();
    }

    public void ConfigureSshSerial(SshModule module)
    {
        if (_connection is not ConnectSerial) return;

        var config = new List<string>(File.ReadAllLines(Paths.Ssh));

        config = ParameterReplacer.ReplaceParameter(config, "${username}$",
            !string.IsNullOrEmpty(module.Username) ? module.Username : "admin");
        config = ParameterReplacer.ReplaceParameter(config, "${password}$",
            !string.IsNullOrEmpty(module.Password) ? module.Password : "admin");

        config = ParameterReplacer.ReplaceParameter(config, "${secret}$",
            !string.IsNullOrEmpty(module.NewSecret) ? module.NewSecret : "passpass");
        config = ParameterReplacer.ReplaceParameter(config, "${hostname}$",
            !string.IsNullOrEmpty(module.Hostname) ? module.Hostname : "CCFM");
        config = ParameterReplacer.ReplaceParameter(config, "${domain}$",
            !string.IsNullOrEmpty(module.DomainName) ? module.DomainName : "ccfm.local");
        config = ParameterReplacer.ReplaceParameter(config, "${ipwithmask}$",
            !string.IsNullOrEmpty(module.IpAddressWithMask) && CommandValidator.IsIpAddress(module.IpAddressWithMask)
                ? module.IpAddressWithMask
                : "192.168.0.2 255.255.255.0");

        _connection.Secret = module.OldSecret;
        _connection.SendModule(config);

        _connection.Secret = module.NewSecret;
        _connection.CopyToStartupConfig();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}