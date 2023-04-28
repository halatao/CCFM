using CiscoConfigurationFileManager.Models;
using CiscoConfigurationFileManager.Services.Configurations;
using Reactive.Bindings;
using System;

namespace CiscoConfigurationFileManager.ViewModels;

public class SerialModulesViewModel
{
    private readonly ConfigurationsManager? _configurationsManager;

    /// <summary>
    /// Initializes a new instance of the SerialModulesViewModel class with the specified COM port.
    /// </summary>
    /// <param name="comPort">The name of the COM port to use.</param>
    public SerialModulesViewModel(string? comPort)
    {
        _configurationsManager = new ConfigurationsManager();
        _configurationsManager.SetupSerial(comPort);

        Init();
    }

    /// <summary>
    /// Initializes a new instance of the SerialModulesViewModel class.
    /// </summary>
    public SerialModulesViewModel()
    {
    }

    public ReactiveCommand? ConfigureSshCommand { get; private set; }
    public ReactiveCommand? NavigateBack { get; private set; }
    public ReactiveCommand? GoToConnections { get; private set; }

    public ReactiveProperty<bool>? SshModuleValid { get; set; }
    public ReactiveProperty<string?>? Username { get; set; }
    public ReactiveProperty<string>? Password { get; set; }
    public ReactiveProperty<string>? NewSecret { get; set; }
    public string OldSecret { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string DomainName { get; set; } = string.Empty;
    public string IpAddressWithMask { get; set; } = string.Empty;

    /// <summary>
    /// Initializes the view model.
    /// </summary>
    public void Init()
    {
        Username = new ReactiveProperty<string>();
        Password = new ReactiveProperty<string>();
        NewSecret = new ReactiveProperty<string>();
        SshModuleValid = new ReactiveProperty<bool>(false);

        ConfigureSshCommand = new ReactiveCommand().WithSubscribe(ConfigureSshSerial);
        GoToConnections = new ReactiveCommand().WithSubscribe(NavigateToConnections);
        NavigateBack = new ReactiveCommand().WithSubscribe(NavigateToBack);

        Username.Subscribe(q => { ValidateSshModule(); });
        Password.Subscribe(q => { ValidateSshModule(); });
        NewSecret.Subscribe(q => { ValidateSshModule(); });
    }

    /// <summary>
    /// Validates the SSH module and updates the SshModuleValid property.
    /// </summary>
    private void ValidateSshModule()
    {
        if (!String.IsNullOrEmpty(Username?.Value) && !String.IsNullOrEmpty(Password?.Value) &&
            !String.IsNullOrEmpty(NewSecret?.Value) && SshModuleValid != null)
            SshModuleValid.Value = true;
    }

    /// <summary>
    /// Configures the SSH module using the specified settings.
    /// </summary>
    public void ConfigureSshSerial()
    {
        _configurationsManager?.ConfigureSshSerial(new SshModule
        {
            Username = Username?.Value,
            Password = Password?.Value,
            OldSecret = OldSecret,
            NewSecret = NewSecret?.Value,
            Hostname = Hostname,
            DomainName = DomainName,
            IpAddressWithMask = IpAddressWithMask,
        });
    }

    /// <summary>
    /// Closes the serial connection.
    /// </summary>
    public void CloseConnection()
    {
        _configurationsManager?.Dispose();
    }

    /// <summary>
    /// Navigates to the Connections view.
    /// </summary>
    public void NavigateToConnections()
    {
        CloseConnection();
        NavigationViewModel.NavigateToConnection();
    }

    // <summary>
    /// Navigates back to the previous view.
    /// </summary>
    public void NavigateToBack()
    {
        CloseConnection();
        NavigationViewModel.NavigateToPreviousView();
    }
}