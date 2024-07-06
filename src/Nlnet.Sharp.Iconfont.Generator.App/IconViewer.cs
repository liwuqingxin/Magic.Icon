using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace Nlnet.Sharp;

public class IconViewer : Window
{
    private List<Iconfont> Iconfonts { get; set; }

    public IconViewer()
    {
        Iconfonts = new List<Iconfont>()
        {
            new("", "", "", "", Enum.GetValues<IconKeys>().Select(v => new IconItem("", v.ToString(), IconExtension.Values[v])).ToList()),
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

        //this.Content = listBox;

        BuildTemplate();
    }

    private void BuildTemplate()
    {
        var tabControl = new TabControl
        {
            ItemsSource = Iconfonts
        };

        this.Content = tabControl;
    }



    private class IconItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public IconItem(string id, string name, string icon)
        {
            Id = id;
            Name = name;
            Icon = icon;
        }
    }

    private class Iconfont
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string FontFamily { get; set; }

        public IList<IconItem> Icons { get; set; }

        public Iconfont(string id, string description, string name, string fontFamily, IList<IconItem> icons)
        {
            Id = id;
            Description = description;
            Name = name;
            FontFamily = fontFamily;
            Icons = icons;
        }

        public override string ToString()
        {
            return $"{Name}, FontFamily: {FontFamily}, {Icons.Count}";
        }
    }
}