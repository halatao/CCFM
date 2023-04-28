using System;
using System.IO;

namespace CiscoConfigurationFileManager;

/// <summary>
/// Contains static paths to various directories and files used by the application.
/// </summary>
public static class Paths
{
    /// <summary>
    /// Gets the root directory of the application.
    /// </summary>
    public static readonly string? RootDirectory =
        Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;

    /// <summary>
    /// Gets the directory path for TFTP configurations.
    /// </summary>
    public static readonly string TftpDirectory = RootDirectory + "\\Configurations";

    /// <summary>
    /// Gets the file path for the running configuration downloaded via TFTP.
    /// </summary>
    public static readonly string TftpRunningConfig = TftpDirectory + "\\Download.txt";

    /// <summary>
    /// Gets the file path for the configuration to be uploaded via TFTP.
    /// </summary>
    public static readonly string TftpConfigToUpload = TftpDirectory + "\\Upload.txt";

    /// <summary>
    /// Gets the file path for the TFTP download commands.
    /// </summary>
    public static readonly string TftpDownloadCommands = TftpDirectory + "\\downloadCommands.txt";

    /// <summary>
    /// Gets the file path for the TFTP upload commands.
    /// </summary>
    public static readonly string TftpUploadCommands = TftpDirectory + "\\uploadCommands.txt";

    /// <summary>
    /// Gets the file path for the example file.
    /// </summary>
    public static readonly string Example = TftpDirectory + "\\example.txt";

    /// <summary>
    /// Gets the file path for the clean file.
    /// </summary>
    public static readonly string Clean = TftpDirectory + "\\clean.txt";

    /// <summary>
    /// Gets the file path for the SSH file.
    /// </summary>
    public static readonly string Ssh = TftpDirectory + "\\ssh.txt";

    /// <summary>
    /// Gets the file path for the regular expressions used by the application.
    /// </summary>
    public static readonly string Regex = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\regex.txt";

    /// <summary>
    /// Gets the file path for the categories used by the application.
    /// </summary>
    public static readonly string Categories = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\categories.json";

    /// <summary>
    /// Gets the directory path for the user's documents.
    /// </summary>
    public static readonly string UserDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
}