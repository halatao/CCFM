using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CiscoConfigurationFileManager.Services.Connections;

/// <summary>
/// Represents a serial port connection to a Cisco device.
/// </summary>
public class ConnectSerial : Connection
{
    private readonly int _bitRate;
    private readonly string? _comPort;
    private readonly IMessenger _messenger = WeakReferenceMessenger.Default;
    private SerialPort? _serialPort;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectSerial"/> class with the specified COM port and bit rate.
    /// </summary>
    /// <param name="comPort">The COM port to use for the connection.</param>
    /// <param name="bitRate">The bit rate to use for the connection.</param>
    public ConnectSerial(string? comPort, int bitRate)
    {
        _comPort = comPort;
        _bitRate = bitRate;
        Log = new List<string?>();
    }

    /// <summary>
    /// Gets the log of the connection.
    /// </summary>
    public List<string?> Log { get; }

    /// <inheritdoc/>
    public override void Connect()
    {
        if (_serialPort is {IsOpen: true}) return;
        _serialPort = new SerialPort(_comPort, _bitRate);
        _serialPort.DataReceived += ResponseReceived;
        _serialPort.Open();
    }

    /// <inheritdoc/>
    public override void SendCommand(string? command)
    {
        _serialPort?.WriteLine($"{command}\r");
    }

    /// <inheritdoc/>
    public override async void SendModule(List<string> module)
    {
        CheckConfMode = true;
        SendCommand("");

        await AwaitConfMode();

        foreach (var line in module)
        {
            SendCommand(line);
        }
    }

    /// <inheritdoc/>
    public override async Task<List<string>> RetrieveConfig()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void UploadConfig(List<string>? config)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        _serialPort?.Close();
        _serialPort?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void ResponseReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var response = _serialPort?.ReadLine();
        if (CheckConfMode)
            SetConfigurationMode(response);
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new SetTextDelegate(WriteData), response);
    }

    private void WriteData(string text)
    {
    }

    private delegate void SetTextDelegate(string text);
}