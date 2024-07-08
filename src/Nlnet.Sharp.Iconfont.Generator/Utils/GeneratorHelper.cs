using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Nlnet.Sharp
{
    internal static class GeneratorHelper
    {
        private static readonly char[] InvalidChars = new[]
        {
            ' ', '-', '.', '?', '+', '=', '*', '/', '\\', '!', '@', 
            '#', '$', '%', '^', '&', ',', '~', '`', ';', ':',
            '<', '>', '"', '\'', '|', '(', ')', '[', ']', '{', '}',
        };


        public static string AsName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var chars = name.ToArray();
            for (var i = 0; i < chars.Length; i++)
            {
                var ch = chars[i];

                if (InvalidChars.Contains(ch))
                {
                    chars[i] = '_';
                }
            }

            if (chars[0] >= '0' && chars[0] <= '9')
            {
                return $"_{new string(chars)}";
            }

            return new string(chars);
        }

        public static Location AsLocation(this string path)
        {
            return Location.Create(path, new TextSpan(), new LinePositionSpan());
        }
    }
}
