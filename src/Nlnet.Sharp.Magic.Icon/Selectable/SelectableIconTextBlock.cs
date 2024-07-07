using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Sharp
{
    public class SelectableIconTextBlock : SelectableTextBlock
    {
        public string? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly StyledProperty<string?> IconProperty = AvaloniaProperty
            .Register<SelectableIconTextBlock, string?>(nameof(Icon));

        public new string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public new static readonly StyledProperty<string?> TextProperty = AvaloniaProperty
            .Register<SelectableIconTextBlock, string?>(nameof(Text));

        public double? IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        public static readonly StyledProperty<double?> IconSizeProperty = AvaloniaProperty
            .Register<SelectableIconTextBlock, double?>(nameof(IconSize), null);

        public IBrush? IconBrush
        {
            get { return GetValue(IconBrushProperty); }
            set { SetValue(IconBrushProperty, value); }
        }
        public static readonly StyledProperty<IBrush?> IconBrushProperty = AvaloniaProperty
            .Register<SelectableIconTextBlock, IBrush?>(nameof(IconBrush), null);

        public string? Separator
        {
            get => GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }
        public static readonly StyledProperty<string?> SeparatorProperty = AvaloniaProperty
            .Register<SelectableIconTextBlock, string?>(nameof(Separator), "\u00a0\u00a0");

        public BaselineAlignment BaselineAlignment
        {
            get { return GetValue(BaselineAlignmentProperty); }
            set { SetValue(BaselineAlignmentProperty, value); }
        }
        public static readonly StyledProperty<BaselineAlignment> BaselineAlignmentProperty = AvaloniaProperty
            .Register<SelectableIconTextBlock, BaselineAlignment>(nameof(BaselineAlignment), BaselineAlignment.Center);



        static SelectableIconTextBlock()
        {
            IconProperty.Changed.AddClassHandler<SelectableIconTextBlock>((text, args) => text.Update());
            TextProperty.Changed.AddClassHandler<SelectableIconTextBlock>((text, args) => text.Update());
            IconSizeProperty.Changed.AddClassHandler<SelectableIconTextBlock>((text, args) => text.Update());
            IconBrushProperty.Changed.AddClassHandler<SelectableIconTextBlock>((text, args) => text.Update());
            SeparatorProperty.Changed.AddClassHandler<SelectableIconTextBlock>((text, args) => text.Update());
            BaselineAlignmentProperty.Changed.AddClassHandler<SelectableIconTextBlock>((text, args) => text.Update());
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
