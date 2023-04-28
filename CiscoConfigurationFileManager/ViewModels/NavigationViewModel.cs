using CiscoConfigurationFileManager.ViewModels.Connection;
using Reactive.Bindings;

namespace CiscoConfigurationFileManager.ViewModels;

/// <summary>
/// Provides methods for navigating between different views in the application.
/// </summary>
public class NavigationViewModel
{
    public static ReactiveProperty<object> CurrentView { get; set; }
    public static object PreviousView { get; set; }

    static NavigationViewModel()
    {
        CurrentView = new ReactiveProperty<object>(new ConnectionViewModel());
    }

    /// <summary>
    /// Navigates to the Serial Modules view.
    /// </summary>
    /// <param name="comPort">The COM port to connect to.</param>
    public static void NavigateToSerialModule(string? comPort)
    {
        PreviousView = CurrentView.Value;
        CurrentView!.Value = new SerialModulesViewModel(comPort);
    }

    /// <summary>
    /// Navigates to the SSH Modules view.
    /// </summary>
    /// <param name="ip">The IP address to connect to.</param>
    /// <param name="username">The SSH username to use.</param>
    /// <param name="password">The SSH password to use.</param>
    /// <param name="secret">The SSH secret to use.</param>
    public static void NavigateToSshModule(string? ip, string? username, string? password, string? secret)
    {
        PreviousView = CurrentView.Value;
        CurrentView.Value = new ModuleViewModel(ip, username, password, secret);
    }

    /// <summary>
    /// Navigates to the File Module view.
    /// </summary>
    /// <param name="path">The path to the file to load.</param>
    public static void NavigateToFileModule(string path)
    {
        PreviousView = CurrentView.Value;
        CurrentView.Value = new ModuleViewModel(path);
    }

    /// <summary>
    /// Navigates to the Connection view.
    /// </summary>
    public static void NavigateToConnection()
    {
        PreviousView = CurrentView.Value;
        CurrentView.Value = new ConnectionViewModel();
    }

    /// <summary>
    /// Navigates to the SSH configuration view.
    /// </summary>
    public static void NavigateToConfigSsh()
    {
        PreviousView = CurrentView.Value;
        CurrentView.Value = new ConfigSshViewModel();
    }

    /// <summary>
    /// Navigates to the Serial configuration view.
    /// </summary>
    public static void NavigateToConfigSerial()
    {
        PreviousView = CurrentView.Value;
        CurrentView.Value = new ConfigSerialViewModel();
    }

    /// <summary>
    /// Navigates to the previous view.
    /// </summary>
    public static void NavigateToPreviousView()
    {
        CurrentView.Value = PreviousView;
    }
}