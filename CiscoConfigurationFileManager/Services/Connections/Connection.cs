using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiscoConfigurationFileManager.Services.Connections;

/// <summary>
/// Abstract class representing a connection to a network device. Implements IDisposable.
/// </summary>
public abstract class Connection : IDisposable
{
    public bool CheckConfMode = true;
    public string? Secret { get; set; } = string.Empty;

    /// <summary>
    /// Sets the configuration mode based on the latest response received.
    /// </summary>
    /// <param name="latestResponse">The latest response received from the device.</param>
    public void SetConfigurationMode(string? latestResponse)
    {
        if (latestResponse!.Contains("(config)#"))
        {
            CheckConfMode = false;
            return;
        }

        if (latestResponse.Contains(">"))
        {
            SendCommand("en");
        }
        else if (!latestResponse.Contains("config") && latestResponse.Contains("#"))
        {
            SendCommand("conf t");
        }

        if (latestResponse!.Contains("Password:"))
        {
            SendCommand(Secret);
        }
    }

    /// <summary>
    /// Copies the running configuration to the startup configuration.
    /// </summary>
    public async void CopyToStartupConfig()
    {
        CheckConfMode = true;
        SendCommand("");

        await AwaitConfMode();
        SendCommand("end");
        SendCommand("copy running-config startup-config");
    }

    /// <summary>
    /// Waits for the device to enter configuration mode.
    /// </summary>
    public async Task AwaitConfMode()
    {
        while (CheckConfMode)
        {
            await Task.Delay(50);
        }
    }

    /// <summary>
    /// Disposes of the object.
    /// </summary>
    public virtual void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Connects to the Cisco device.
    /// </summary>
    public virtual void Connect()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends a command to the Cisco device.
    /// </summary>
    /// <param name="command">The command to send.</param>
    public virtual void SendCommand(string? command)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends a module of configuration commands to the Cisco device.
    /// </summary>
    /// <param name="module">The module to send.</param>
    public virtual void SendModule(List<string> module)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves the configuration of the Cisco device.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public virtual Task<List<string>> RetrieveConfig()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uploads the configuration to the Cisco device.
    /// </summary>
    /// <param name="config">The configuration to upload.</param>
    public virtual void UploadConfig(List<string> config)
    {
        throw new NotImplementedException();
    }
}