using Reactive.Bindings;

namespace CiscoConfigurationFileManager.ViewModels;

/// <summary>
/// ViewModel class responsible for managing the connection to a device and sending commands to it.
/// </summary>
public class ConnectionViewModel
{
    /// <summary>
    /// Initializes a new instance of the ConnectionViewModel class.
    /// </summary>
    public ConnectionViewModel()
    {
        ;
        GoToSsh = new ReactiveCommand().WithSubscribe(NavigateToSsh);
        GoToSerial = new ReactiveCommand().WithSubscribe(NavigateToSerial);
        GoToFile = new ReactiveCommand().WithSubscribe(NavigateToFile);
    }

    public ReactiveCommand GoToSsh { get; }
    public ReactiveCommand GoToSerial { get; }
    public ReactiveCommand GoToFile { get; }

    /// <summary>
    /// Navigates to the serial configuration view.
    /// </summary>
    private void NavigateToSerial()
    {
        NavigationViewModel.NavigateToConfigSerial();
    }

    /// <summary>
    /// Navigates to the SSH configuration view.
    /// </summary>
    private void NavigateToSsh()
    {
        NavigationViewModel.NavigateToConfigSsh();
    }

    /// <summary>
    /// Navigates to the file module view.
    /// </summary>
    private void NavigateToFile()
    {
        NavigationViewModel.NavigateToFileModule(Paths.UserDir);
    }
}