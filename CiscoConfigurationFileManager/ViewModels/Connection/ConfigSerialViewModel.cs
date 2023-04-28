using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;

namespace CiscoConfigurationFileManager.ViewModels.Connection;

/// <summary>
/// View model for the serial connection configuration.
/// </summary>
public class ConfigSerialViewModel
{
    public ReactiveCommand GoToModules { get; set; }
    public ReactiveCommand? GoToConnections { get; private set; }
    public ReactiveCommand? NavigateBack { get; private set; }

    public ReactiveProperty<string?> SelectedPort { get; set; }
    public ReactiveProperty<List<string>> ComPorts { get; set; }
    public ReactiveProperty<bool> Selected { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigSerialViewModel"/> class.
    /// </summary>
    public ConfigSerialViewModel()
    {
        GoToModules = new ReactiveCommand().WithSubscribe(NavigateToModules);
        GoToConnections = new ReactiveCommand().WithSubscribe(GoToConnection);
        NavigateBack = new ReactiveCommand().WithSubscribe(GoToPreviousView);

        Selected = new ReactiveProperty<bool>(false);
        ComPorts = new ReactiveProperty<List<string>>();
        SelectedPort = new ReactiveProperty<string?>();

        ComPorts.Value = SerialPort.GetPortNames().ToList();
        SelectedPort.Subscribe(s =>
        {
            if (!string.IsNullOrEmpty(SelectedPort.Value))
                Selected.Value = true;
        });
    }

    /// <summary>
    /// Navigates to the serial modules view
    /// </summary>
    private void NavigateToModules()
    {
        NavigationViewModel.NavigateToSerialModule(SelectedPort.Value);
    }

    /// <summary>
    /// Navigates to the connections view
    /// </summary>
    private void GoToConnection()
    {
        NavigationViewModel.NavigateToConnection();
    }

    /// <summary>
    /// Navigates back to the previous view
    /// </summary>
    private void GoToPreviousView()
    {
        NavigationViewModel.NavigateToPreviousView();
    }
}