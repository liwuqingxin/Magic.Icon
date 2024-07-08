using Avalonia;
using Avalonia.Media;

namespace Nlnet.Sharp
{
    public class IconVisual : AvaloniaObject
    {
        public static string? GetIcon(Visual host)
        {
            return host.GetValue(IconProperty);
        }
        public static void SetIcon(Visual host, string? value)
        {
            host.SetValue(IconProperty, value);
        }
        public static readonly AttachedProperty<string?> IconProperty = AvaloniaProperty
            .RegisterAttached<IconVisual, Visual, string?>("Icon");

        public static double GetIconSize(Visual host)
        {
            return host.GetValue(IconSizeProperty);
        }
        public static void SetIconSize(Visual host, double value)
        {
            host.SetValue(IconSizeProperty, value);
        }
        public static readonly AttachedProperty<double> IconSizeProperty = AvaloniaProperty
            .RegisterAttached<IconVisual, Visual, double>("IconSize", 16);

        public static IBrush? GetIconBrush(Visual host)
        {
            return host.GetValue(IconBrushProperty);
        }
        public static void SetIconBrush(Visual host, IBrush? value)
        {
            host.SetValue(IconBrushProperty, value);
        }
        public static readonly AttachedProperty<IBrush?> IconBrushProperty = AvaloniaProperty
            .RegisterAttached<IconVisual, Visual, IBrush?>("IconBrush");

        public static FontFamily? GetIconFamily(Visual host)
        {
            return host.GetValue(IconFamilyProperty);
        }
        public static void SetIconFamily(Visual host, FontFamily? value)
        {
            host.SetValue(IconFamilyProperty, value);
        }
        public static readonly AttachedProperty<FontFamily?> IconFamilyProperty = AvaloniaProperty
            .RegisterAttached<IconVisual, Visual, FontFamily?>("IconFamily");
    }
}
