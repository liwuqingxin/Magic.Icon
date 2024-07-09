using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Sharp
{
    public class IconTextBlock : TextBlock, IIconVisual
    {
        public string? Icon
        {
            get => GetValue(IconVisual.IconProperty);
            set => SetValue(IconVisual.IconProperty, value);
        }

        public double IconSize
        {
            get => GetValue(IconVisual.IconSizeProperty);
            set => SetValue(IconVisual.IconSizeProperty, value);
        }

        public IBrush? IconBrush
        {
            get => GetValue(IconVisual.IconBrushProperty);
            set => SetValue(IconVisual.IconBrushProperty, value);
        }

        public FontFamily? IconFamily
        {
            get => GetValue(IconVisual.IconFamilyProperty);
            set => SetValue(IconVisual.IconFamilyProperty, value);
        }

        public new string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public new static readonly StyledProperty<string?> TextProperty = AvaloniaProperty
            .Register<IconTextBlock, string?>(nameof(Text));

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
            IconVisual.IconProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            IconVisual.IconSizeProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            IconVisual.IconBrushProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            IconVisual.IconFamilyProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
            TextProperty.Changed.AddClassHandler<IconTextBlock>((text, args) => text.Update());
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
                    FontSize = IconSize,
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
