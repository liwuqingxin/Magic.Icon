using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable IDE0270

namespace Nlnet.Sharp
{
    [Generator]
    public class IconfontGenerator : ISourceGenerator
    {
        private const string Indent = "    ";
        private static readonly string Nl = Environment.NewLine;



        private static class MsBuildProperties
        {
            public const string FontNamespace = nameof(FontNamespace);

            public const string IconName = nameof(IconName);

            public const string IconNamespace = nameof(IconNamespace);

            public const string AutoFontFamily = nameof(AutoFontFamily);

            public const string InjectFallbackFont = nameof(InjectFallbackFont);

            public const string UseDefaultXmlnsPrefix = nameof(UseDefaultXmlnsPrefix);
        }

        private class IconfontContext
        {
            public IconfontContext(
                GeneratorExecutionContext context, 
                AdditionalText file, 
                IconJson iconJson, 
                string name, 
                string ns, 
                bool autoFontFamily,
                bool injectFallbackFont,
                bool useDefaultXmlnsPrefix)
            {
                Context = context;
                File = file;
                IconJson = iconJson;
                Name = name;
                Namespace = ns;
                AutoFontFamily = autoFontFamily;
                InjectFallbackFont = injectFallbackFont;
                UseDefaultXmlnsPrefix = useDefaultXmlnsPrefix;
            }

            public IconJson IconJson { get; }

            public GeneratorExecutionContext Context { get; }

            public AdditionalText File { get; }
            
            public string Name { get; }

            public string Namespace { get; }

            public bool AutoFontFamily { get; }

            public bool InjectFallbackFont { get; }

            public bool UseDefaultXmlnsPrefix { get; }

            private string JsonFileName => Path.GetFileNameWithoutExtension(File.Path);

            public string FallbackFontInjectorName => $"FallbackFontInjectorFor_{JsonFileNameAndId.AsName()}";

            public static string FallbackFontInjectorInitialize => "Initialize";

            public string JsonFileNameAndId => $"{JsonFileName}.{IconJson.id}";

            public string FontFamilyUriString => GetFontFamilyUriString();

            private string GetFontFamilyUriString()
            {
                var projectDir = Context.GetMsBuildProperty("projectdir");
                var relativeDir = Path.GetDirectoryName(File.Path.Replace(projectDir, ""));
                var fontFamilyUriString = $"avares://{Context.Compilation.AssemblyName}/{relativeDir}/{JsonFileName}.ttf#{IconJson.font_family}";
                return fontFamilyUriString;
            }
        }



        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //Debugger.Launch();

            if (!(context.Compilation is CSharpCompilation))
            {
                throw new Exception($"{nameof(IconfontGenerator)} only support C#.");
            }

            context.OptionsDiagnostic();

            var ctxs = new List<IconfontContext>();

            try
            {
                foreach (var file in context.AdditionalFiles.Where(f => Path.GetExtension(f.Path).ToLower() == ".json"))
                {
                    Trace.WriteLine($"[Nlnet.Sharp.Iconfont.Generator] Additional Json File: {file.Path}");

                    // IconName
                    var iconName = context.GetMsBuildFileMetadata(file, MsBuildProperties.IconName);
                    if (string.IsNullOrWhiteSpace(iconName))
                    {
                        continue;
                    }

                    // Namespace.
                    var ns = context.GetMsBuildFileMetadata(file, MsBuildProperties.IconNamespace);
                    if (string.IsNullOrWhiteSpace(ns))
                    {
                        ns = context.GetDefaultNamespace();
                    }

                    // Options
                    var autoFontFamily = context.GetBoolProperty(file, MsBuildProperties.AutoFontFamily);
                    var injectFallbackFont = context.GetBoolProperty(file, MsBuildProperties.InjectFallbackFont);
                    var useDefaultXmlnsPrefix = context.GetBoolProperty(file, MsBuildProperties.UseDefaultXmlnsPrefix);

                    // Json.
                    var json = file.GetText(context.CancellationToken)?.ToString();
                    if (json == null){ throw new Exception("The content of the json is empty.");}

                    // Icon json object.
                    var iconJson = json.DeserializeByNsj<IconJson>();
                    if (iconJson.name == null) throw new Exception("The 'font_family' of the json is empty.");
                    if (iconJson.font_family == null) throw new Exception("The 'font_family' of the json is empty.");
                    if (iconJson.glyphs == null) throw new Exception("The 'glyphs' of the json is empty.");

                    // IconJson Context.
                    var ctx = new IconfontContext(context, file, iconJson, iconName, ns, autoFontFamily, injectFallbackFont, useDefaultXmlnsPrefix);
                    ctxs.Add(ctx);

                    // UseDefaultXmlnsPrefix.
                    if (ctx.UseDefaultXmlnsPrefix)
                    {
                        BuildDefaultXmlnsPrefix(ctx);
                    }

                    // InjectFallbackFont.
                    if (ctx.InjectFallbackFont)
                    {
                        BuildFallbackFontInjector(ctx);
                    }

                    BuildInformation(ctx);
                    BuildClass(ctx);
                    BuildMarkup(ctx);
                }

                BuildFontFamilyMarkup(context, ctxs);
            }
            catch (Exception e)
            {
                context.ReportError(
                    id: "NSIG001", 
                    title: "Unexpected Exception for Building Iconfont Class", 
                    message: $"{e.Message}.", 
                    description: "Please file an issue if you think it is a bug.",
                    helpLinkUri: "https://www.devtools.nlnet.net");
            }
        }

        private static void BuildFontFamilyMarkup(GeneratorExecutionContext context, IList<IconfontContext> ctxs)
        {
            var fontKeysName = $"SharpIconFamilyKeys";
            var fontMarkupName = $"SharpIconFamilyExtension";
            var ns = GetFontNamespace(context);

            Build(ctxs, context, fontKeysName, builder =>
            {
                builder.AppendLine($"using System;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ns};");
                builder.AppendLine();
                builder.AppendLine($"public enum {fontKeysName}{Nl}{{");

                foreach (var ctx in ctxs)
                {
                    builder.AppendLine($"{Indent}{ctx.Name.AsName()},");
                }

                builder.AppendLine($"}}");
            });

            Build(ctxs, context, fontMarkupName, builder =>
            {
                builder.AppendLine($"using System;");
                builder.AppendLine($"using System.Collections.Generic;");
                builder.AppendLine($"using Avalonia.Markup.Xaml;");
                builder.AppendLine($"using Avalonia.Media;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ns};");
                builder.AppendLine();
                builder.AppendLine($"public class {fontMarkupName} : MarkupExtension{Nl}{{");
                builder.AppendLine();

                builder.AppendLine($"{Indent}public static readonly IReadOnlyDictionary<{fontKeysName}, FontFamily> Values = new Dictionary<{fontKeysName}, FontFamily>()");
                builder.AppendLine($"{Indent}{{");

                foreach (var ctx in ctxs)
                {
                    builder.AppendLine($"{Indent}{Indent}{{ {fontKeysName}.{ctx.Name.AsName()}, FontFamily.Parse(\"{ctx.FontFamilyUriString}\") }},");
                }

                builder.AppendLine($"{Indent}}};");
                builder.AppendLine();
                builder.AppendLine($"{Indent}private readonly {fontKeysName} _key;");
                builder.AppendLine();
                builder.AppendLine($"{Indent}public {fontMarkupName}({fontKeysName} key) => _key = key;");
                builder.AppendLine();
                builder.AppendLine(@"
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Values.TryGetValue(_key, out var v))
        {
            return v;
        }

        return null;
    }
");
                builder.AppendLine($"}};");
            });
        }

        private static void BuildInformation(IconfontContext ctx)
        {
            var entityName = ctx.Name.AsName();
            var infoName = $"{entityName}Info";

            Build(ctx, infoName, builder =>
            {
                builder.AppendLine($"using System;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ctx.Namespace};");
                builder.AppendLine();
                builder.AppendLine($@"
public static class {infoName} 
{{
    public const string Id = ""{ctx.IconJson.id}"";
    
    public const string Name = ""{ctx.IconJson.name}"";

    public const string FontFamily = ""{ctx.IconJson.font_family}"";

    public const string Description = ""{ctx.IconJson.description}"";

    public const int Count = {ctx.IconJson.glyphs.Count};
}}
");
            });
        }

        private static void BuildFallbackFontInjector(IconfontContext ctx)
        {
            var fileName = $"FallbackFontInjector";
            var fontFamilyUriString = ctx.FontFamilyUriString;
            
            Build(ctx, fileName, builder =>
            {
                builder.AppendLine($@"
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Media;

namespace {ctx.Namespace};

internal static class {ctx.FallbackFontInjectorName}
{{
    public static void {IconfontContext.FallbackFontInjectorInitialize}() {{ }}
    
    static {ctx.FallbackFontInjectorName}() => Inject();

    private static void Inject()
    {{
        var field = typeof(FontManager).GetField(""_fontFallbacks"", BindingFlags.Instance | BindingFlags.NonPublic);
        var fallbacks = (field?.GetValue(FontManager.Current) as FontFallback[])?.ToList() ?? new List<FontFallback>();
        fallbacks.Add(new FontFallback() {{ FontFamily = FontFamily.Parse(""{fontFamilyUriString}"") }});
        field?.SetValue(FontManager.Current, fallbacks);
    }}
}}
");
            });
        }

        private static void BuildDefaultXmlnsPrefix(IconfontContext ctx)
        {
            var fileName = $"XmlnsPrefix";
            Build(ctx, fileName, builder =>
            {
                builder.AppendLine($"using Avalonia.Metadata;");
                builder.AppendLine();
                builder.AppendLine($"[assembly: XmlnsDefinition(\"https://github.com/avaloniaui\", \"{ctx.Namespace}\")]");
            });
        }

        private static void BuildClass(IconfontContext ctx)
        {
            var entityName = ctx.Name.AsName();
            var className = $"{entityName}Chars";

            Build(ctx, className, (builder) =>
            {
                builder.AppendLine($"using System;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ctx.Namespace};");
                builder.AppendLine();
                builder.AppendLine($"public class {className}{Nl}{{");
                
                if (ctx.InjectFallbackFont)
                {
                    AppendInitializingFallbackFontInjector(builder, ctx, className);
                    builder.AppendLine();
                }

                foreach (var glyph in ctx.IconJson.glyphs)
                {
                    builder.AppendLine($"{Indent}/// <summary>");
                    builder.AppendLine($"{Indent}/// [{glyph.icon_id}] {glyph.font_class}");
                    builder.AppendLine($"{Indent}/// </summary>");
                    builder.AppendLine($"{Indent}public const string {glyph.name.AsName()} = \"\\x{glyph.unicode}\";");
                }

                builder.AppendLine($"}}");
            });
        }

        private static void BuildMarkup(IconfontContext ctx)
        {
            var entityName = ctx.Name.AsName();
            var markupKeysName = $"{entityName}Keys";
            var markupName = $"{entityName}Extension";

            Build(ctx, markupKeysName, (builder) =>
            {
                builder.AppendLine($"using System;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ctx.Namespace};");
                builder.AppendLine();
                builder.AppendLine($"public enum {markupKeysName}{Nl}{{");

                foreach (var glyph in ctx.IconJson.glyphs)
                {
                    builder.AppendLine($"{Indent}{glyph.name.AsName()},");
                }

                builder.AppendLine($"}}");
            });

            var fontNs = GetFontNamespace(ctx.Context);

            Build(ctx, markupName, (builder) =>
            {
                builder.AppendLine($"using System;");
                builder.AppendLine($"using System.Collections.Generic;");
                builder.AppendLine($"using Avalonia.Markup.Xaml;");
                builder.AppendLine($"using Avalonia.Controls;");
                builder.AppendLine($"using Avalonia.Controls.Documents;");
                builder.AppendLine($"using {fontNs};");
                builder.AppendLine();
                builder.AppendLine($"namespace {ctx.Namespace};");
                builder.AppendLine();
                builder.AppendLine($"public class {markupName} : MarkupExtension{Nl}{{");

                if (ctx.InjectFallbackFont)
                {
                    AppendInitializingFallbackFontInjector(builder, ctx, markupName);
                    builder.AppendLine();
                }

                builder.AppendLine();
                builder.AppendLine($"{Indent}public static readonly IReadOnlyDictionary<{markupKeysName}, string> Values = new Dictionary<{markupKeysName}, string>()");
                builder.AppendLine($"{Indent}{{");

                foreach (var glyph in ctx.IconJson.glyphs)
                {
                    builder.AppendLine($"{Indent}{Indent}{{ {markupKeysName}.{glyph.name.AsName()}, \"\\x{glyph.unicode}\" }},");
                }

                builder.AppendLine($"{Indent}}};");
                builder.AppendLine();
                builder.AppendLine($"{Indent}private readonly {markupKeysName} _key;");
                builder.AppendLine();
                builder.AppendLine($"{Indent}public {markupName}({markupKeysName} key) => _key = key;");
                builder.AppendLine();

                var autoFontFamily = string.Empty;
                if (ctx.AutoFontFamily)
                {
                    autoFontFamily = $@"
        var targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (targetProvider?.TargetObject is Control control)
        {{
            TextElement.SetFontFamily(control, SharpIconFamilyExtension.Values[SharpIconFamilyKeys.{ctx.Name.AsName()}]);
        }}
";
                }

                builder.AppendLine($@"
    public override object ProvideValue(IServiceProvider serviceProvider)
    {{
{autoFontFamily}

        if (Values.TryGetValue(_key, out var v))
        {{
            return v;
        }}

        return _key;
    }}
");
                builder.AppendLine($"}};");
            });
        }

        private static void Build(IconfontContext ctx, string entityName, Action<StringBuilder> contentBuilder)
        {
            try
            {
                var builder = new StringBuilder();

                AppendFontInfoHeader(builder, ctx);

                contentBuilder(builder);
                ctx.Context.AddSource($"{ctx.JsonFileNameAndId}.{entityName}.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
            catch (Exception e)
            {
                ctx.Context.ReportError("NSIG002", "Invalid Icon Json File", $"{e.Message}", "", "", Location.Create(ctx.File.Path, new TextSpan(), new LinePositionSpan()));
            }
        }

        private static void Build(ICollection<IconfontContext> ctxs, GeneratorExecutionContext context, string entityName, Action<StringBuilder> contentBuilder)
        {
            try
            {
                var builder = new StringBuilder();

                AppendGlobalInfoHeader(builder, ctxs);

                contentBuilder(builder);
                context.AddSource($"Nlnet.Sharp.Iconfont.Generator.{entityName}.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
            catch (Exception e)
            {
                context.ReportError("NSIG002", "Unexpected Exception", $"{e.Message}", "", "");
            }
        }

        private static void AppendFontInfoHeader(StringBuilder builder, IconfontContext ctx)
        {
            builder.AppendLine($"// Nlnet Iconfont Generator: https://github.com/liwuqingxin/Iconfont.Generator");
            builder.AppendLine($"// Version: {AssemblyVersionProvider.GetVersion<IconfontGenerator>()}");
            builder.AppendLine($"// ");
            builder.AppendLine($"// This file is generated by Nlnet Iconfont Generator.");
            builder.AppendLine($"// Id:          {ctx.IconJson.id}");
            builder.AppendLine($"// Name:        {ctx.IconJson.name}");
            builder.AppendLine($"// FontFamily:  {ctx.IconJson.font_family}");
            builder.AppendLine($"// Description: {ctx.IconJson.description}");
            builder.AppendLine($"// Count:       {ctx.IconJson.glyphs.Count}");
            builder.AppendLine();
        }

        private static void AppendGlobalInfoHeader(StringBuilder builder, ICollection<IconfontContext> ctxs)
        {
            builder.AppendLine($"// Nlnet Iconfont Generator: https://github.com/liwuqingxin/Iconfont.Generator");
            builder.AppendLine($"// Version: {AssemblyVersionProvider.GetVersion<IconfontGenerator>()}");
            builder.AppendLine($"// ");
            builder.AppendLine($"// This file is generated by Nlnet Iconfont Generator.");
            builder.AppendLine($"// Font Count: {ctxs.Count}");
            builder.AppendLine();
        }

        private static void AppendInitializingFallbackFontInjector(StringBuilder builder, IconfontContext ctx, string ctor)
        {
            builder.AppendLine($"{Indent}static {ctor}() => {ctx.FallbackFontInjectorName}.{IconfontContext.FallbackFontInjectorInitialize}();");
        }

        private static string GetFontNamespace(GeneratorExecutionContext context)
        {
            var ns = context.GetMsBuildProperty(MsBuildProperties.FontNamespace);
            if (string.IsNullOrWhiteSpace(ns))
            {
                ns = context.GetDefaultNamespace();
            }

            return ns;
        }
    }
}
