using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Nlnet.Sharp.Iconfont.Generator
{
    internal class IconJson
    {
        public string id { get; set; }

        public string name { get; set; }

        public string font_family { get; set; }

        public string description { get; set; }

        public IList<Glyph> glyphs { get; set; }

        public override string ToString()
        {
            return $"{name}, FontFamily: {font_family}, {glyphs.Count}";
        }
    }

    internal class Glyph
    {
        public string icon_id { get; set; }

        public string name { get; set; }

        public string font_class { get; set; }

        public string unicode { get; set; }

        public int unicode_decimal { get; set; }
    }
}