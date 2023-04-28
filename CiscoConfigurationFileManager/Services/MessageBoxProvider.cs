using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CiscoConfigurationFileManager.Services;

/// <summary>
/// Provides methods to display message boxes with custom buttons and messages.
/// </summary>
public class MessageBoxProvider
{
    private static Window CreateWindow(string title, int width, int height)
    {
        var window = new Window
        {
            Title = title,
            Width = width,
            Height = height,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.ToolWindow,
            ShowInTaskbar = false,
            Topmost = true
        };
        return window;
    }

    private static Button CreateButton(string label)
    {
        var button = new Button
        {
            Content = label,
            Margin = new Thickness(10),
            Width = 75,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Center // Align button to center horizontally
        };
        return button;
    }

    public static TextBlock CreateTextBlock(string text)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(10),
            FontSize = 15
        };
        return textBlock;
    }

    public static void ShowYesNo(string message, string title, string yesButtonLabel, string noButtonLabel,
        Action<bool> action)
    {
        var window = CreateWindow(title, 400, 250);

        var grid = new Grid();

        var row1 = new RowDefinition();
        var row2 = new RowDefinition();
        row2.Height = new GridLength(1, GridUnitType.Auto);

        grid.RowDefinitions.Add(row1);
        grid.RowDefinitions.Add(row2);

        var textBlock = CreateTextBlock(message);

        var yesButton = CreateButton(yesButtonLabel);

        var noButton = CreateButton(noButtonLabel);

        yesButton.Click += (sender, args) =>
        {
            action(true);
            window.Close();
        };

        noButton.Click += (sender, args) =>
        {
            action(false);
            window.Close();
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10),
        };

        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);

        Grid.SetRow(textBlock, 0);
        Grid.SetRow(buttonPanel, 1);

        grid.Children.Add(textBlock);
        grid.Children.Add(buttonPanel);

        window.Content = grid;
        window.ShowDialog();
    }

    public static void ShowOk(string message, string title, string okButtonLabel, Action action)
    {
        var window = CreateWindow(title, 400, 250);

        var grid = new Grid();

        var row1 = new RowDefinition();
        var row2 = new RowDefinition();
        row2.Height = new GridLength(1, GridUnitType.Auto);

        grid.RowDefinitions.Add(row1);
        grid.RowDefinitions.Add(row2);

        var textBlock = CreateTextBlock(message);

        var okButton = CreateButton(okButtonLabel);

        okButton.Click += (sender, args) =>
        {
            action();
            window.Close();
        };

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10),
        };

        panel.Children.Add(okButton);

        Grid.SetRow(textBlock, 0);
        Grid.SetRow(panel, 1);

        grid.Children.Add(textBlock);
        grid.Children.Add(panel);

        window.Content = grid;
        window.ShowDialog();
    }

    public static void ShowError(string message, string title)
    {
        var window = CreateWindow(title, 400, 250);

        var grid = new Grid();

        var row1 = new RowDefinition();
        var row2 = new RowDefinition();
        row2.Height = new GridLength(1, GridUnitType.Auto);

        grid.RowDefinitions.Add(row1);
        grid.RowDefinitions.Add(row2);

        var textBlock = CreateTextBlock(message);

        var okButton = CreateButton("Ok");

        okButton.Click += (sender, args) => { window.Close(); };

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10),
        };

        panel.Children.Add(okButton);

        Grid.SetRow(textBlock, 0);
        Grid.SetRow(panel, 1);

        grid.Children.Add(textBlock);
        grid.Children.Add(panel);

        window.Content = grid;
        window.ShowDialog();
    }

    public static void ShowAbout(string title, string okButtonLabel)
    {
        var window = CreateWindow(title, 400, 250);

        var grid = new Grid();

        var row1 = new RowDefinition();
        var row2 = new RowDefinition();
        row2.Height = new GridLength(1, GridUnitType.Auto);

        grid.RowDefinitions.Add(row1);
        grid.RowDefinitions.Add(row2);

        var textBlock = CreateTextBlock("");

        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock.VerticalAlignment = VerticalAlignment.Center;

        var issues = new Hyperlink
        {
            NavigateUri = new Uri("https://github.com/halatao/CCFM/issues"),
            Inlines = {new Run("Report issues")}
        };

        issues.RequestNavigate += (sender, e) =>
        {
            Process.Start(new ProcessStartInfo {FileName = e.Uri.AbsoluteUri, UseShellExecute = true});
            e.Handled = true;
        };

        var author = new Hyperlink
        {
            NavigateUri = new Uri("https://github.com/halatao"),
            Inlines = {new Run("Author")}
        };

        author.RequestNavigate += (sender, e) =>
        {
            Process.Start(new ProcessStartInfo {FileName = e.Uri.AbsoluteUri, UseShellExecute = true});
            e.Handled = true;
        };

        var repository = new Hyperlink
        {
            NavigateUri = new Uri("https://github.com/halatao/CCFM"),
            Inlines = {new Run("Repository")}
        };

        repository.RequestNavigate += (sender, e) =>
        {
            Process.Start(new ProcessStartInfo {FileName = e.Uri.AbsoluteUri, UseShellExecute = true});
            e.Handled = true;
        };

        textBlock.Inlines.Add(author);
        textBlock.Inlines.Add(new LineBreak());
        textBlock.Inlines.Add(repository);
        textBlock.Inlines.Add(new LineBreak());
        textBlock.Inlines.Add(issues);
        textBlock.Margin = new Thickness(10);

        var okButton = new Button
        {
            Content = okButtonLabel,
            Margin = new Thickness(10),
            Width = 75,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        okButton.Click += (sender, args) => { window.Close(); };

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10),
        };

        panel.Children.Add(okButton);

        Grid.SetRow(textBlock, 0);
        Grid.SetRow(panel, 1);

        grid.Children.Add(textBlock);
        grid.Children.Add(panel);

        window.Content = grid;
        window.ShowDialog();
    }
}