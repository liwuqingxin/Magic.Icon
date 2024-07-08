using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Nlnet.Sharp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewer = new IconViewer();
        viewer.ShowDialog(this);
    }

    private void ButtonVisitWebsite_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string s })
        {
            return;
        }

        Process.Start(new ProcessStartInfo(s) { UseShellExecute = true });
    }
}