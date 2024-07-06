using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace Nlnet.Sharp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var viewer = new IconViewer();
        viewer.Show();
    }
}

public class IconViewer : Window
{
    private class IconItem
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public IconItem(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }
    }

    private class Iconfont
    {
        public List<IconItem> Icons { get; set; }

        public Iconfont(List<IconItem> icons)
        {
            Icons = icons;
        }
    }

    private List<Iconfont> Iconfonts { get; set; }

    public IconViewer()
    {
        Iconfonts = new List<Iconfont>()
        {
            new(Enum.GetValues<IconKeys>().Select(v => new IconItem(v.ToString(), IconExtension.Values[v])).ToList()),
        };

        var listBox = new ListBox
        {
            ItemsSource = Iconfonts[0].Icons,
            ItemTemplate = new FuncDataTemplate<IconItem?>((data, s) =>
                {
                    var result = new TextBlock()
                    {
                        FontSize = 96,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    result.Bind(TextBlock.TextProperty, new Binding(nameof(IconItem.Icon)));
                    return result;
                },
                true),
            ItemsPanel = new FuncTemplate<Panel?>(() => new WrapPanel()
            {
                Orientation = Orientation.Horizontal,
                ItemHeight = 120,
                ItemWidth = 120,
            }),
        };

        this.Content = listBox;
    }

    private static TextBlock CreateIcon(string icon)
    {
        var textBlock= new TextBlock
        {
            Text = icon,
            FontSize = 96,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        return textBlock;
    }
}