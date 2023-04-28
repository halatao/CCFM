using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CiscoConfigurationFileManager.Services.Connections;

/// <summary>
/// Represents a connection to a flat file for retrieving and uploading configurations.
/// </summary>
public class ConnectFlatFile : Connection
{
    public ConnectFlatFile()
    {
    }

    public override void Connect()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Retrieves a list of strings representing the configuration stored in the flat file asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of strings representing the configuration.</returns>
    public override async Task<List<string>> RetrieveConfig()
    {
        return new List<string>(await File.ReadAllLinesAsync(Paths.Clean));
    }

    /// <summary>
    /// Uploads the provided configuration to the flat file specified in Paths.TftpConfigToUpload.
    /// </summary>
    /// <param name="config">A list of strings representing the configuration to be uploaded.</param>
    public override void UploadConfig(List<string> config)
    {
        var conf = Paths.TftpConfigToUpload;
        if (!File.Exists(conf))
        {
            File.Create(conf).Close();
        }

        File.WriteAllLines(conf, config);
    }
}