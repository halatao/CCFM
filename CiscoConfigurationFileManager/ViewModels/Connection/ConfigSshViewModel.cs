using CiscoConfigurationFileManager.Services;
using CiscoConfigurationFileManager.Services.Configurations;
using Reactive.Bindings;
using System;

namespace CiscoConfigurationFileManager.ViewModels.Connection;

/// <summary>
/// ViewModel for SSH configuration page
/// </summary>
public class ConfigSshViewModel
{
    public ReactiveCommand GoToModules { get; set; }
    public ReactiveCommand? GoToConnections { get; private set; }
    public ReactiveCommand? NavigateBack { get; private set; }

    public ReactiveProperty<string> IpAddress { get; set; }
    public ReactiveProperty<bool> IpValid { get; set; }

    /// <summary>
    /// Credentials for SSH authentication
    /// </summary>
    public string? Username { get; set; }

    public string? Password { get; set; }
    public string? Secret { get; set; }

    /// <summary>
    /// Constructor for ConfigSshViewModel
    /// </summary>
    public ConfigSshViewModel()
    {
        IpAddress = new ReactiveProperty<string>("");
        IpValid = new ReactiveProperty<bool>(false);
        IpAddress.Subscribe(q => { IpValid.Value = CommandValidator.IsIpAddress(IpAddress.Value); });

        GoToModules = new ReactiveCommand().WithSubscribe(NavigateToModules);
        GoToConnections = new ReactiveCommand().WithSubscribe(GoToConnection);
        NavigateBack = new ReactiveCommand().WithSubscribe(GoToPreviousView);
    }

    /// <summary>
    /// Navigate to the SSH module page
    /// </summary>
    private void NavigateToModules()
    {
        try
        {
            NavigationViewModel.NavigateToSshModule(IpAddress.Value, Username, Password, Secret);
        }
        catch (Exception ex)
        {
            CreateException.ShowExceptionMessage(ex, "Error while connecting:\n", false);
        }
    }

    /// <summary>
    /// Navigate to the connection page
    /// </summary>
    private void GoToConnection()
    {
        NavigationViewModel.NavigateToConnection();
    }

    /// <summary>
    /// Navigate back to the previous page
    /// </summary>
    private void GoToPreviousView()
    {
        NavigationViewModel.NavigateToPreviousView();
    }
}