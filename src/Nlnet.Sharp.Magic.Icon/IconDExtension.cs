//using System;
//using Avalonia.Controls.Documents;
//using Avalonia.Controls;
//using Avalonia.Markup.Xaml;

//namespace Nlnet.Sharp
//{
//    public class IconDExtension : MarkupExtension
//    {
//        public string Prefix { get; set; }

//        public string Suffix { get; set; }

//        public bool SpaceBetween { get; set; } = true;

//        public bool AutoSetFontFamily { get; set; } = true;

//        private const string Space = "\u00a0";

//        public override object ProvideValue(IServiceProvider serviceProvider)
//        {
//            if (AutoSetFontFamily)
//            {
//                var targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

//                if (targetProvider?.TargetObject is Control control)
//                {
//                    TextElement.SetFontFamily(control, SharpIconFamilyExtension.Values[SharpIconFamilyKeys.IconD]);
//                }
//                else if (targetProvider?.TargetObject is TextElement element)
//                {
//                    element.SetCurrentValue(TextElement.FontFamilyProperty,
//                        SharpIconFamilyExtension.Values[SharpIconFamilyKeys.IconD]);
//                }
//            }

//            Values.TryGetValue(_key, out var v);

//            return Concatenate(Space, Prefix, v, Suffix);
//        }

//        private static string Concatenate(string separator, params string[] strings)
//        {
//            var builder = new StringBuilder();

//            foreach (var s in strings)
//            {
//                if (string.IsNullOrEmpty(s))
//                {
//                    continue;
//                }

//                builder.Append(s);
//                builder.Append(separator);
//            }

//            if (builder.Length > 0)
//            {
//                builder.Remove(builder.Length - 1, 1);
//            }

//            return builder.ToString();
//        }

//    }
//}
