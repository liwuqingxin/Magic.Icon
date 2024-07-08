using Avalonia.Media;

namespace Nlnet.Sharp
{
    public interface IMagicIconHost
    {
        public string? Icon { get; set; }

        public double IconSize { get; set; }

        public IBrush? IconBrush { get; set; }

        public FontFamily? IconFamily { get; set; }  
    }
}
