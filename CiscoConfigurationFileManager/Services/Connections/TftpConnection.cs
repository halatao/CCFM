using System;
using System.IO;
using System.Net;
using Tftp.Net;

namespace CiscoConfigurationFileManager.Services.Connections;

/// <summary>
/// Represents a connection to a TFTP server.
/// </summary>
public class TftpConnection : IDisposable
{
    public string? ServerDirectory;
    private static readonly Lazy<TftpConnection> Inst = new(() => new TftpConnection());
    private readonly TftpServer _server;

    /// <summary>
    /// Creates a new instance of the TftpConnection class.
    /// </summary>
    private TftpConnection()
    {
        ServerDirectory = Paths.TftpDirectory;
        _server = new TftpServer();
        _server.OnReadRequest += new TftpServerEventHandler(OnReadRequest);
        _server.OnWriteRequest += new TftpServerEventHandler(OnWriteRequest);
        _server.Start();
    }

    /// <summary>
    /// Gets the singleton instance of the TftpConnection class.
    /// </summary>
    public static TftpConnection Instance => Inst.Value;

    /// <summary>
    /// Handles a write request from a TFTP client.
    /// </summary>
    /// <param name="transfer">The transfer object representing the transfer.</param>
    /// <param name="client">The endpoint of the client.</param>
    private void OnWriteRequest(ITftpTransfer transfer, EndPoint client)
    {
        var file = Path.Combine(ServerDirectory!, transfer.Filename);

        if (File.Exists(file))
        {
            CancelTransfer(transfer, TftpErrorPacket.FileAlreadyExists);
        }
        else
        {
            OutputTransferStatus(transfer, "Accepting write request from " + client);
            StartTransfer(transfer, new FileStream(file, FileMode.CreateNew));
        }
    }

    /// <summary>
    /// Handles a read request from a TFTP client.
    /// </summary>
    /// <param name="transfer">The transfer object representing the transfer.</param>
    /// <param name="client">The endpoint of the client.</param>
    private void OnReadRequest(ITftpTransfer transfer, EndPoint client)
    {
        var path = Path.Combine(ServerDirectory!, transfer.Filename);
        var file = new FileInfo(path);

        if (!file.FullName.StartsWith(ServerDirectory!, StringComparison.InvariantCultureIgnoreCase))
        {
            CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
        }
        else if (!file.Exists)
        {
            CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
        }
        else
        {
            OutputTransferStatus(transfer, "Accepting request from " + client);
            StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
        }
    }

    /// <summary>
    /// Starts a TFTP transfer.
    /// </summary>
    /// <param name="transfer">The transfer object representing the transfer.</param>
    /// <param name="stream">The stream to read from or write to.</param>
    private static void StartTransfer(ITftpTransfer transfer, Stream stream)
    {
        transfer.OnProgress += new TftpProgressHandler(OnProgress);
        transfer.OnError += new TftpErrorHandler(OnError);
        transfer.OnFinished += new TftpEventHandler(OnFinished);
        transfer.Start(stream);
    }

    /// <summary>
    /// Cancels a TFTP transfer.
    /// </summary>
    /// <param name="transfer">The transfer object representing the transfer.</param>
    /// <param name="reason">The reason for cancelling the transfer.</param>
    private static void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
    {
        OutputTransferStatus(transfer, "Cancelling transfer: " + reason.ErrorMessage);
        transfer.Cancel(reason);
    }

    /// <summary>
    /// Handles a TFTP transfer error.
    /// </summary>
    static void OnError(ITftpTransfer transfer, TftpTransferError error)
    {
        OutputTransferStatus(transfer, "Error: " + error);
    }

    /// <summary>
    /// Handles the completion of a TFTP transfer.
    /// </summary>
    static void OnFinished(ITftpTransfer transfer)
    {
        OutputTransferStatus(transfer, "Finished");
    }

    /// <summary>
    /// Handles progress updates for a TFTP transfer.
    /// </summary>
    static void OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
    {
        OutputTransferStatus(transfer, "Progress " + progress);
    }

    /// <summary>
    /// Logs TFTP transfer.
    /// </summary>
    private static void OutputTransferStatus(ITftpTransfer transfer, string message)
    {
        Console.WriteLine("[" + transfer.Filename + "] " + message);
    }

    /// <summary>
    /// Dispose TFTP server.
    /// </summary>
    public void Dispose()
    {
        _server?.Dispose();
    }
}