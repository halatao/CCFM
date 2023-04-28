using CiscoConfigurationFileManager.ViewModels;
using System;

namespace CiscoConfigurationFileManager.Services;

/// <summary>
/// Static class for creating and displaying error messages to the user, with the option to navigate back to the previous view.
/// </summary>
public static class CreateException
{
    public static void ShowExceptionMessage(Exception ex, string message, bool navigateBack)
    {
        MessageBoxProvider.ShowError(message + ex.Message, "Error");
        if (navigateBack)
            NavigationViewModel.NavigateToPreviousView();
    }
}