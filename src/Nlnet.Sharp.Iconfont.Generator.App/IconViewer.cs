using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace Nlnet.Sharp;

public class IconViewer : Window
{
    private List<Iconfont> Iconfonts { get; set; }

    public IconViewer()
    {
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Width = 990;
        Height = 720;

        Iconfonts = new List<Iconfont>()
        {
            new("id", "description", "name", "fontFamily",
                ColorIconExtension.Values.Select(v => new IconItem("", v.Key.ToString(), v.Value)).ToList()),
            new("id", "description", "name", "fontFamily",
                IconExtension.Values.Select(v => new IconItem("", v.Key.ToString(), v.Value)).ToList()),
        };

        this.Content = CreateIconfontTabControl();
    }

    private Control CreateIconfontTabControl()
    {
        var tabControl = new TabControl
        {
            ItemsSource = Iconfonts,
            ItemTemplate = new FuncDataTemplate<Iconfont?>((data, s) =>
            {
                var textBlock = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                textBlock.Bind(TextBlock.TextProperty, new Binding(nameof(Iconfont.Name)));

                return textBlock;
            }, true),
            ContentTemplate = new FuncDataTemplate<Iconfont?>((data, s) =>
            {
                var listBox = CreateIconfontListControl();
                listBox.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(Iconfont.Icons)));
                return listBox;
            }, true),
        };

        return tabControl;
    }

    private static ListBox CreateIconfontListControl()
    {
        var listBox = new ListBox
        {
            Margin = new Thickness(3),
            Background = null,
            ItemTemplate = new FuncDataTemplate<IconItem?>((data, s) =>
            {
                var textBlock = new TextBlock()
                {
                    FontSize = 48,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                textBlock.Bind(TextBlock.TextProperty, new Binding(nameof(IconItem.Icon)));

                var button = new Button()
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 10,
                    Background = Brushes.Transparent,
                    Cursor = new Cursor(StandardCursorType.Hand)
                };
                TextBlock.SetTextTrimming(button, TextTrimming.CharacterEllipsis);
                button.Bind(Button.ContentProperty, new Binding(nameof(IconItem.Name)));

                var panel = new Panel();
                panel.Children.Add(textBlock);
                panel.Children.Add(button);

                return panel;
            }, true),
            ItemsPanel = new FuncTemplate<Panel?>(() => new WrapPanel()
            {
                Orientation = Orientation.Horizontal,
                ItemHeight = 120,
                ItemWidth = 120,
            }),
        };

        // Basic style.
        listBox.Styles.Add(new Style(selector => selector.OfType<ListBoxItem>())
        {
            Setters =
            {
                new Setter(MarginProperty, new Thickness(3)),
                new Setter(PaddingProperty, new Thickness(3)),
                new Setter(BorderThicknessProperty, new Thickness(1)),
                new Setter(BorderBrushProperty, Brushes.LightGray),
                new Setter(CornerRadiusProperty, new CornerRadius(6)),
            },
        });

        // Hover color.
        listBox.Styles.Add(new Style(selector => selector.OfType<ListBoxItem>().Template().OfType<ContentPresenter>().Name("PART_ContentPresenter"))
        {
            Setters =
            {
                new Setter(TransitionsProperty, new Transitions()
                {
                    new BrushTransition()
                    {
                        Property = ContentPresenter.BackgroundProperty,
                        Duration = TimeSpan.FromMilliseconds(200),
                        Easing = Easing.Parse("0.5,0.8,0.5,0.8"),
                    }
                })
            }
        });
        listBox.Styles.Add(new Style(selector => selector.OfType<ListBoxItem>().Class(":pointerover").Template().OfType<ContentPresenter>().Name("PART_ContentPresenter"))
        {
            Setters = { new Setter(BackgroundProperty, Brushes.LightGray), }
        });
        listBox.Styles.Add(new Style(selector => selector.OfType<ListBoxItem>().Class(":selected").Template().OfType<ContentPresenter>().Name("PART_ContentPresenter"))
        {
            Setters = { new Setter(BackgroundProperty, Brushes.LightBlue), }
        });

        // Hover to scale.
        listBox.Styles.Add(new Style(selector => selector.OfType<ListBoxItem>().Descendant().OfType<TextBlock>())
        {
            Setters =
            {
                new Setter(RenderTransformProperty, TransformOperations.Identity),
                new Setter(TransitionsProperty, new Transitions()
                {
                    new TransformOperationsTransition()
                    {
                        Property = Border.RenderTransformProperty,
                        Duration = TimeSpan.FromMilliseconds(200),
                        Easing = Easing.Parse("0.5,0.8,0.5,0.8"),
                    }
                })
            }
        });
        listBox.Styles.Add(new Style(selector => selector.OfType<ListBoxItem>().Class(":pointerover").Descendant().OfType<TextBlock>())
        {
            Setters = { new Setter(RenderTransformProperty, TransformOperations.Parse("scale(1.3,1.3)")), }
        });
        listBox.Styles.Add(new Style(selector => selector.OfType<ListBoxItem>().Class(":selected").Descendant().OfType<TextBlock>())
        {
            Setters = { new Setter(RenderTransformProperty, TransformOperations.Parse("scale(1.3,1.3)")), }
        });

        return listBox;
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
            return $"{Name}, FontFamily: {FontFamily}, Count: {Icons.Count}";
        }
    }
}