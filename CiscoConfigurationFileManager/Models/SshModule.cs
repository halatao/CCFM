namespace CiscoConfigurationFileManager.Models;

/// <summary>
/// A class representing an SSH module, which contains various properties such as the username and password for authentication,
/// the old and new secret keys, the hostname, domain name, and IP address with mask.
/// </summary>
public class SshModule
{
    public string? Username { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public string? OldSecret { get; set; } = string.Empty;
    public string? NewSecret { get; set; } = string.Empty;
    public string? Hostname { get; set; } = string.Empty;
    public string? DomainName { get; set; } = string.Empty;
    public string IpAddressWithMask { get; set; } = string.Empty;
}