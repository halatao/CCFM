using CiscoConfigurationFileManager.Models;
using CiscoConfigurationFileManager.Services;
using CiscoConfigurationFileManager.Services.Configurations;
using Microsoft.Win32;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CiscoConfigurationFileManager.ViewModels;

/// <summary>
/// Represents a view model for a module.
/// </summary>
public class ModuleViewModel : IDisposable
{
    private readonly ConfigurationsManager? _configurationsManager;

    /// <summary>
    /// Gets or sets the list of running modules.
    /// </summary>
    public List<RunningModule>? Modules { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleViewModel"/> class with SSH credentials.
    /// </summary>
    /// <param name="ip">The IP address.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="secret">The secret.</param>
    public ModuleViewModel(string? ip, string? username, string? password, string? secret)
    {
        _configurationsManager = new ConfigurationsManager();
        _configurationsManager.SetupSsh(ip, username, password, secret);

        Init();
        InitializeAsync().ConfigureAwait(false);

        if (MergeEnabled != null) MergeEnabled.Value = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleViewModel"/> class with a flat file path.
    /// </summary>
    /// <param name="path">The path to the flat file.</param>
    public ModuleViewModel(string? path)
    {
        _configurationsManager = new ConfigurationsManager();
        _configurationsManager.SetupFlatFile();

        Init();
        InitializeAsync().ConfigureAwait(false);

        if (MergeEnabled != null) MergeEnabled.Value = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleViewModel"/> class.
    /// </summary>
    public ModuleViewModel()
    {
    }

    public ReactiveCommand? GoToConnections { get; private set; }
    public ReactiveCommand? SaveChangesModule { get; private set; }
    public ReactiveCommand? SaveChangesModules { get; private set; }
    public ReactiveCommand? MergeConfig { get; private set; }
    public ReactiveCommand? RemoveModule { get; private set; }
    public ReactiveCommand? RemoveLine { get; private set; }
    public ReactiveCommand? AddLine { get; private set; }
    public ReactiveCommand? ExampleToRunning { get; private set; }
    public ReactiveCommand? ShowExample { get; private set; }
    public ReactiveCommand? ShowSelected { get; private set; }
    public ReactiveCommand? FetchInteractiveConfig { get; private set; }
    public ReactiveCommand? AddExampleModule { get; private set; }
    public ReactiveCommand? OpenRegexConfigCommand { get; private set; }
    public ReactiveCommand? OpenCategoryConfigCommand { get; private set; }
    public ReactiveCommand? SaveConfigAsCommand { get; private set; }
    public ReactiveCommand? OpenConfigCommand { get; private set; }
    public ReactiveCommand? NewConfigCommand { get; private set; }
    public ReactiveCommand? NavigateBack { get; private set; }
    public ReactiveCommand? ShowAboutCommand { get; private set; }


    public ReactiveCollection<RunningModule>? InteractiveModules { get; set; }
    public ReactiveCollection<LineWrapper>? InteractiveConfig { get; set; }
    public ReactiveProperty<RunningModule>? SelectedModule { get; set; }
    public ReactiveProperty<ExampleModule?>? SelectedExampleModule { get; private set; }
    public ReactiveProperty<StringWrapper?>? SelectedExampleModuleToAdd { get; private set; }
    public ReactiveProperty<List<StringWrapper?>>? ExampleModules { get; set; }
    public ReactiveProperty<bool>? ExampleShowed { get; set; }
    public ReactiveProperty<bool>? SelectedShowed { get; set; }
    public ReactiveProperty<bool>? IsExampleModuleToAddSelected { get; set; }
    public static ReactiveProperty<bool>? MergeEnabled { get; set; }

    /// <summary>
    /// Releases all resources used by the <see cref="ModuleViewModel"/> object.
    /// </summary>
    public void Dispose()
    {
        GoToConnections?.Dispose();
        SaveChangesModule?.Dispose();
        SaveChangesModules?.Dispose();
        MergeConfig?.Dispose();
        RemoveModule?.Dispose();
        AddLine?.Dispose();
        ExampleToRunning?.Dispose();
        ShowExample?.Dispose();
        ShowSelected?.Dispose();
        FetchInteractiveConfig?.Dispose();
        SelectedModule?.Dispose();
        SelectedExampleModule?.Dispose();
        ExampleShowed?.Dispose();
        SelectedShowed?.Dispose();
        InteractiveConfig?.Dispose();
    }

    /// <summary>
    /// Initializes the class and sets up the reactive properties and commands.
    /// </summary>
    public void Init()
    {
        ExampleModules = new ReactiveProperty<List<StringWrapper?>>();
        GoToConnections = new ReactiveCommand().WithSubscribe(NavigationViewModel.NavigateToConnection);
        SaveChangesModule = new ReactiveCommand().WithSubscribe(UpdateModule);
        SaveChangesModules = new ReactiveCommand().WithSubscribe(UpdateModules);
        MergeConfig = new ReactiveCommand().WithSubscribe(MergeConfiguration);
        RemoveModule = new ReactiveCommand().WithSubscribe(DeleteSelectedModule);
        AddLine = new ReactiveCommand().WithSubscribe(AddEmptyLine);
        ExampleToRunning = new ReactiveCommand().WithSubscribe(MoveExampleToRunning);
        ShowExample = new ReactiveCommand().WithSubscribe(ShowExampleConfiguration);
        ShowSelected = new ReactiveCommand().WithSubscribe(ShowSelectedConfiguration);
        FetchInteractiveConfig = new ReactiveCommand().WithSubscribe(FetchInteractiveConfiguration);
        AddExampleModule = new ReactiveCommand().WithSubscribe(AddExampleModuleToModules);
        RemoveLine = (ReactiveCommand?) new ReactiveCommand().WithSubscribe(DeleteLineCommand!);
        SaveConfigAsCommand = new ReactiveCommand().WithSubscribe(SaveAsModules);
        OpenConfigCommand = new ReactiveCommand().WithSubscribe(OpenModules);
        NewConfigCommand = new ReactiveCommand().WithSubscribe(CreateNewModules);
        NavigateBack = new ReactiveCommand().WithSubscribe(NavigationViewModel.NavigateToPreviousView);
        SaveConfigAsCommand = new ReactiveCommand().WithSubscribe(SaveAsModules);
        OpenConfigCommand = new ReactiveCommand().WithSubscribe(OpenModules);
        NewConfigCommand = new ReactiveCommand().WithSubscribe(CreateNewModules);
        OpenRegexConfigCommand = (ReactiveCommand?) new ReactiveCommand().WithSubscribe(q =>
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Paths.Regex,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                CreateException.ShowExceptionMessage(ex, "Error while opening file:", false);
            }
        });
        OpenCategoryConfigCommand = (ReactiveCommand?) new ReactiveCommand().WithSubscribe(q =>
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Paths.Categories,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                CreateException.ShowExceptionMessage(ex, "Error while opening file:", true);
            }
        });
        ShowAboutCommand = (ReactiveCommand?) new ReactiveCommand().WithSubscribe(q =>
        {
            MessageBoxProvider.ShowAbout("About", "Ok");
        });


        InteractiveModules = new ReactiveCollection<RunningModule>();
        SelectedModule = new ReactiveProperty<RunningModule>();
        SelectedExampleModule = new ReactiveProperty<ExampleModule?>();
        SelectedExampleModuleToAdd = new ReactiveProperty<StringWrapper?>();
        ExampleShowed = new ReactiveProperty<bool>(false);
        SelectedShowed = new ReactiveProperty<bool>(false);
        IsExampleModuleToAddSelected = new ReactiveProperty<bool>(false);
        InteractiveConfig = new ReactiveCollection<LineWrapper>();
        MergeEnabled = new ReactiveProperty<bool>(false);

        SelectedExampleModuleToAdd.Subscribe(q =>
        {
            IsExampleModuleToAddSelected.Value = !String.IsNullOrEmpty(q?.String);
        });

        InteractiveModules.CollectionChanged += (sender, args) =>
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
            }
        };
    }

    /// <summary>
    /// Initializes the view model and downloads the configuration from the server.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            await DownloadConfig();

            // Set up the ExampleModules property with the names of the example modules from the configuration.
            if (ExampleModules != null)
                ExampleModules.Value = (_configurationsManager?.Configuration?.ExampleModules
                                            .Select(q => new StringWrapper {String = q.Name}).ToList() ??
                                        new List<StringWrapper>())!;

            // Set up the ExampleModules property with the names of the example modules from the configuration.
            if (ExampleModules != null)
                ExampleModules.Value =
                    (_configurationsManager?.Configuration?.ExampleModulesNames ?? new List<StringWrapper>())!;
        }
        catch (Exception ex)
        {
            // Display an error message if there was an error downloading the configuration.
            CreateException.ShowExceptionMessage(ex, "Error while downloading configuration", true);
        }
    }

    /// <summary>
    /// Downloads the configuration from the server.
    /// </summary>
    public async Task DownloadConfig()
    {
        await _configurationsManager?.DownloadConfig()!;

        // Set up the Modules property with the running modules from the configuration.
        Modules = _configurationsManager?.Configuration?.RunningModules;

        // Fetch the interactive modules for the UI.
        FetchInteractiveModules();
    }

    /// <summary>
    /// Merges the current configuration with the server configuration and uploads it.
    /// </summary>
    public void MergeConfiguration()
    {
        // Update the running modules with the current configuration.
        UpdateModules();

        // Upload the updated configuration to the server.
        _configurationsManager?.UploadConfig();
    }

    /// <summary>
    /// Updates the selected module with the current configuration and refreshes the UI.
    /// </summary>
    public void UpdateModule()
    {
        if (SelectedModule == null) return;

        // Get the selected module.
        var module = SelectedModule.Value;

        // Update the module's configuration with the current interactive configuration.
        var toAdd = (InteractiveConfig ?? new ReactiveCollection<LineWrapper>()).ToList();
        module.Configuration = toAdd;

        // Replace the old module with the updated module.
        Modules?.Remove(Modules?.Find(q => q.Name == module.Name) ?? new RunningModule());
        Modules?.Add(module);

        // Update the selected module and refresh the interactive configuration.
        SelectedModule.Value = module;
        FetchInteractiveConfiguration();
    }


    /// <summary>
    /// Updates the configuration with any changes made to the running modules.
    /// </summary>
    public void UpdateModules()
    {
        // Check if a module is currently selected and being displayed
        if (SelectedShowed?.Value == true)
            UpdateModule(); // Update the selected module before saving changes

        _configurationsManager?.UpdateConfig(Modules); // Save changes to the configuration

        FetchInteractiveModules(); // Update the list of interactive modules
    }

    /// <summary>
    /// Saves the current configuration as a text file.
    /// </summary>
    public void SaveAsModules()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 1,
            RestoreDirectory = true
        };

        if (dialog.ShowDialog() != true) return; // User cancelled the dialog

        var filePath = dialog.FileName;

        // Update the selected module before saving changes
        if (SelectedModule?.Value != null)
            UpdateModule();

        UpdateModules(); // Save changes to the configuration

        _configurationsManager?.SaveAsConfig(filePath); // Save the configuration to a text file

        // Show a message box confirming the configuration was successfully exported
        MessageBoxProvider.ShowOk("Configuration exported", "Configuration exported", "Ok", () => { });
    }

    /// <summary>
    /// Opens a configuration file and replaces the current configuration with the new one.
    /// </summary>
    public void OpenModules()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt",
            FilterIndex = 1,
            RestoreDirectory = true
        };

        if (dialog.ShowDialog() != true) return; // User cancelled the dialog

        var filePath = dialog.FileName;

        if (!File.Exists(filePath)) return; // File not found

        _configurationsManager?.OpenConfig(filePath); // Load the configuration from the text file

        Modules = _configurationsManager?.Configuration?.RunningModules; // Update the running modules list
        FetchInteractiveModules(); // Update the list of interactive modules

        HideSelectedConfiguration(); // Hide the currently displayed module (if any)

        // Show a message box confirming the configuration was successfully imported
        MessageBoxProvider.ShowOk("Configuration imported", "Configuration imported", "Ok", () => { });
    }

    /// <summary>
    /// Creates a new empty configuration.
    /// </summary>
    public void CreateNewModules()
    {
        _configurationsManager?.NewConfig(); // Create a new empty configuration

        Modules = _configurationsManager?.Configuration?.RunningModules; // Update the running modules list
        FetchInteractiveModules(); // Update the list of interactive modules

        HideSelectedConfiguration(); // Hide the currently displayed module (if any)
    }

    /// <summary>
    /// Moves the selected example module to the running module list.
    /// </summary>
    private void MoveExampleToRunning()
    {
        if (SelectedExampleModule?.Value == null)
            return;

        InteractiveConfig?.Clear();
        InteractiveConfig?.AddRangeOnScheduler(
            (SelectedExampleModule.Value.Configuration ?? throw new InvalidOperationException()).Select(q =>
                new LineWrapper {Line = {Value = q.String ?? string.Empty}, Original = false}));
    }

    /// <summary>
    /// Adds the selected example module to the running module list.
    /// </summary>
    public void AddExampleModuleToModules()
    {
        var exampleModule = _configurationsManager?.Configuration?.ExampleModules.Find(q =>
            q.Tag == SelectedExampleModuleToAdd?.Value?.String);
        var config = (exampleModule?.Configuration ?? new List<StringWrapperRead>()).Select(q => new LineWrapper
        {
            Line = {Value = q.String ?? string.Empty},
            OperationEnum = OperationEnum.None,
            Original = false
        });

        if (exampleModule != null) exampleModule.OperationEnum = OperationEnum.Override;

        if (Modules != null && Modules.FindAll(q => q.Name == exampleModule?.Name).Count == 1)
        {
            Modules.Find(q => q.Name == exampleModule?.Name)?.Configuration?
                .AddRange((config));
            Modules.Find(q => q.Name == exampleModule?.Name)!.OperationEnum = OperationEnum.Override;

            FetchInteractiveConfiguration();
            FetchInteractiveModules();

            if (SelectedModule != null)
                SelectedModule.Value = Modules?.Find(q => q.Name == exampleModule?.Name) ?? new RunningModule();
        }
        else
        {
            Modules?.Add(new RunningModule
            {
                Configuration = config.ToList(),
                Name = exampleModule?.Name ?? string.Empty,
                OperationEnum = OperationEnum.Override,
                Tag = exampleModule?.Tag ?? string.Empty
            });
            FetchInteractiveModules();
        }
    }

    /// <summary>
    /// Deletes the selected module from the running module list.
    /// </summary>
    public void DeleteSelectedModule()
    {
        var x = SelectedModule?.Value;
        var module = Modules?.Find(q => q.Name == SelectedModule?.Value.Name);

        if (module?.Configuration == null) return;

        if ((module.Configuration ?? new List<LineWrapper>()).All(q =>
                q.OperationEnum == OperationEnum.Delete) ||
            module.Configuration?.Count == 0)
        {
            Modules?.Remove(module);
            HideSelectedConfiguration();
            UpdateModules();
            FetchInteractiveModules();
            return;
        }

        module.OperationEnum = OperationEnum.Delete;

        var filtered =
            (module.Configuration ?? new List<LineWrapper>()).Where(q => q.OperationEnum != OperationEnum.Delete)
            .ToList();

        filtered.ForEach(line => { line.Operate(OperationEnum.Delete); });

        module.Configuration?.Clear();
        module.Configuration?.AddRange(filtered);

        FetchInteractiveConfiguration();
    }

    /// <summary>
    /// Deletes the selected line from the interactive configuration. If the line is marked for deletion or is not an original line, it is removed from the configuration immediately. Otherwise, the line is marked for deletion and will be removed upon saving changes.
    /// </summary>
    /// <param name="parameter">The line to delete.</param>
    public void DeleteLineCommand(object parameter)
    {
        var item = parameter as LineWrapper ?? new LineWrapper();

        if (item.OperationEnum == OperationEnum.Delete || !item.Original)
        {
            InteractiveConfig?.Remove(item);
            return;
        }

        item.Operate(OperationEnum.Delete);
    }

    /// <summary>
    /// Adds a new empty line to the interactive configuration.
    /// </summary>
    public void AddEmptyLine()
    {
        InteractiveConfig?.Add(new LineWrapper {Line = {Value = ""}, Original = false});
    }

    /// <summary>
    /// Shows a dialog asking the user if they want to discard unsaved changes and update the configuration. If the user chooses to discard changes, the previous view is navigated to. If not, the merge of the current configuration is disabled, and the user is notified that they need to connect again to update the configuration.
    /// </summary>
    public static void ShowChangesDetectedDialog()
    {
        var result = true;
        MessageBoxProvider.ShowYesNo(
            "Changes detected in original configuration. Do you want discard unsaved changes and update configuration?",
            "Changes detected", "Yes", "No",
            action =>
            {
                result = action;
                if (action)
                {
                    NavigationViewModel.NavigateToPreviousView();
                }
                else
                {
                    if (MergeEnabled != null) MergeEnabled.Value = false;
                }
            });

        Thread.Sleep(150);

        if (!result)
        {
            MessageBoxProvider.ShowOk(
                "Newer configuration exists, merge of current configuration disabled. To update configuration connect please again.",
                "Changes detected",
                "Ok",
                () => { Console.WriteLine("OK"); });
        }
    }

    /// <summary>
    /// Toggles the display of the example configuration for the selected module. Sets the selected example module to the example configuration for the selected module.
    /// </summary>
    public void ShowExampleConfiguration()
    {
        ExampleShowed!.Value = !ExampleShowed!.Value;

        var exampleModule =
            _configurationsManager?.Configuration?.ExampleModules.Find(q => q.Tag == SelectedModule?.Value.Tag);


        SelectedExampleModule!.Value = exampleModule;
    }

    /// <summary>
    /// Shows the selected configuration. If the example configuration is currently displayed, it is hidden.
    /// </summary>
    private void ShowSelectedConfiguration()
    {
        if (SelectedModule?.Value == null)
            return;

        if (ExampleShowed != null)
            ExampleShowed.Value = false;

        if (SelectedShowed is {Value: false})
            SelectedShowed.Value = true;
    }

    /// <summary>
    /// Hides the selected configuration.
    /// </summary>
    private void HideSelectedConfiguration()
    {
        if (SelectedShowed is {Value: true})
            SelectedShowed.Value = false;
    }

    /// <summary>
    /// Fetches the interactive configuration for the selected module and displays it.
    /// </summary>
    public void FetchInteractiveConfiguration()
    {
        if (SelectedModule?.Value == null)
            return;
        InteractiveConfig?.Clear();
        InteractiveConfig?.AddRangeOnScheduler(SelectedModule?.Value.Configuration ?? new List<LineWrapper>());
    }

    /// <summary>
    /// Fetches the list of interactive modules and displays them in alphabetical order by tag.
    /// </summary>
    public void FetchInteractiveModules()
    {
        InteractiveModules?.ClearOnScheduler();
        var sorted = (Modules ?? new List<RunningModule>()).OrderBy(q => q.Tag);
        if (Modules != null)
            InteractiveModules?.AddRangeOnScheduler(sorted);
    }
}