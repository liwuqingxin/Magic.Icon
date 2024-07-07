using System;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Nlnet.Sharp
{
    //public class XMarkup : MarkupExtension
    //{
    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        var targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
    //        if (targetProvider?.TargetObject is Control control)
    //        {
    //            TextElement.SetFontFamily(Control, SharpIconFamilyExtension.Values[SharpIconFamilyKeys.Icon]);
    //        }

    //        return string.Empty;
    //    }
    //}

    public class IconTextBlock : TextBlock
    {
        public string? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly StyledProperty<string?> IconProperty = AvaloniaProperty
            .Register<IconTextBlock, string?>(nameof(Icon));

        public new string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public new static readonly StyledProperty<string?> TextProperty = AvaloniaProperty
            .Register<IconTextBlock, string?>(nameof(Text));

        public double? IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        public static readonly StyledProperty<double?> IconSizeProperty = AvaloniaProperty
            .Register<IconTextBlock, double?>(nameof(IconSize), null);

        public IBrush? IconBrush
        {
            get { return GetValue(IconBrushProperty); }
            set { SetValue(IconBrushProperty, value); }
        }
        public static readonly StyledProperty<IBrush?> IconBrushProperty = AvaloniaProperty
            .Register<IconTextBlock, IBrush?>(nameof(IconBrush), null);

        public FontFamily? IconFamily
        {
            get { return GetValue(IconFamilyProperty); }
            set { SetValue(IconFamilyProperty, value); }
        }
        public static readonly StyledProperty<FontFamily?> IconFamilyProperty = AvaloniaProperty
            .Register<IconTextBlock, FontFamily?>(nameof(IconFamily));

        public string? Separator
        {
            get => GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }
        public static readonly StyledProperty<string?> SeparatorProperty = AvaloniaProperty
            .Register<IconTextBlock, string?>(nameof(Separator), "\u00a0\u00a0");

        public BaselineAlignment BaselineAlignment
        {
            get { return GetValue(BaselineAlignmentProperty); }
            set { SetValue(BaselineAlignmentProperty, value); }
        }
        public static readonly StyledProperty<BaselineAlignment> BaselineAlignmentProperty = AvaloniaProperty
            .Register<IconTextBlock, BaselineAlignment>(nameof(BaselineAlignment), BaselineAlignment.Center);



        static IconTextBlock()
        {
            IconProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            TextProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            IconSizeProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            IconBrushProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            IconFamilyProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            SeparatorProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            BaselineAlignmentProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
        }



        private void Update()
        {
            Inlines?.Clear();

            var existText = string.IsNullOrWhiteSpace(Text) == false;

            if (!string.IsNullOrWhiteSpace(Icon))
            {
                var inline = new Run(Icon)
                {
                    BaselineAlignment = BaselineAlignment,
                    FontSize = IconSize ?? FontSize,
                    Foreground = IconBrush ??  Foreground,
                    FontFamily = IconFamily ?? FontFamily,
                };
                Inlines?.Add(inline);

                if (string.IsNullOrEmpty(Separator) == false && existText)
                {
                    Inlines?.Add(Separator);
                }
            }

            if (existText)
            {
                var inline = new Run(Text)
                {
                    BaselineAlignment = BaselineAlignment,
                };
                Inlines?.Add(inline);
            }

            InvalidateVisual();
        }
    }
}
