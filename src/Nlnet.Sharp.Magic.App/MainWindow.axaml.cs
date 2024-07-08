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

    private void ButtonOpenIconfontWebsite_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://www.iconfont.cn/") { UseShellExecute = true });
    }

    private void ButtonOpenProjectUrlWebsite_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/liwuqingxin/Magic.Icon") { UseShellExecute = true });
    }
}