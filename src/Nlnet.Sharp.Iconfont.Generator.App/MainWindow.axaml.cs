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
        var viewer = new IconViewer()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Width = 960,
            Height = 720,
        };
        viewer.ShowDialog(this);
    }
}