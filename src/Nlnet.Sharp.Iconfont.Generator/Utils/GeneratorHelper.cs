using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Nlnet.Sharp.Iconfont.Generator
{
    internal static class GeneratorHelper
    {
        public static string AsName(this string name)
        {
            var chars = name.ToArray();
            for (var i = 0; i < chars.Length; i++)
            {
                var ch = chars[i];
                if (ch >= 'a' && ch <= 'z')
                {
                    continue;
                }
                if (ch >= 'A' && ch <= 'Z')
                {
                    continue;
                }
                if (ch >= '0' && ch <= '9')
                {
                    continue;
                }

                if (ch == '_')
                {
                    continue;
                }
                chars[i] = '_';
            }

            return new string(chars);
        }

        public static Location AsLocation(this string path)
        {
            return Location.Create(path, new TextSpan(), new LinePositionSpan());
        }
    }
}
