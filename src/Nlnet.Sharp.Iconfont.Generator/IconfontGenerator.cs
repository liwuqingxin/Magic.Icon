using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable IDE0270

namespace Nlnet.Sharp.Iconfont.Generator
{
    [Generator]
    public class IconfontGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //Debugger.Launch();

            if (!(context.Compilation is CSharpCompilation csharpCompilation))
            {
                throw new Exception($"{nameof(IconfontGenerator)} only support C#.");
            }

            try
            {
                foreach (var file in context.AdditionalFiles.Where(f => Path.GetExtension(f.Path).ToLower() == ".json"))
                {
                    // Json.
                    var json = file.GetText(context.CancellationToken)?.ToString();
                    if (json == null){ throw new Exception("The content of the json is empty.");}

                    // Icon json object.
                    var iconJson = json.DeserializeByNsj<IconJson>();
                    if (iconJson.glyphs == null) throw new Exception("The 'glyphs' of the json is empty.");
                    if (iconJson.name == null) context.ReportWarning("NSIGW001", "", "The 'name' of the json is empty.", "", "", file.Path.AsLocation());
                    if (iconJson.font_family == null) context.ReportWarning("NSIGW001", "", "The 'font_family' of the json is empty.", "", "", file.Path.AsLocation());

                    // Namespace.
                    var ns = context.GetMsBuildFileMetadata(file, "IconNamespace", null);
                    if (string.IsNullOrWhiteSpace(ns))
                    {
                        ns = context.GetDefaultNamespace();
                    }

                    // UseDefaultXmlnsPrefix.
                    var useDefaultXmlnsPrefix = context.GetBoolProperty(file, "UseDefaultXmlnsPrefix");
                    if (useDefaultXmlnsPrefix)
                    {
                        var fileName = $"{Path.GetFileNameWithoutExtension(file.Path)}.{iconJson.id}.XmlnsPrefix.g.cs";
                        var code     = $"using Avalonia.Metadata;{Environment.NewLine}[assembly: XmlnsDefinition(\"https://github.com/avaloniaui\", \"{ns}\")]";
                        context.AddSource(fileName, SourceText.From(code, Encoding.UTF8));
                    }

                    // IconClass
                    var iconClass = context.GetMsBuildFileMetadata(file, "IconClass");
                    if (string.IsNullOrWhiteSpace(iconClass) == false)
                    {
                        BuildJsonToClass(context, file, iconJson, ns, iconClass);
                    }

                    // IconMarkup
                    var iconMarkup = context.GetMsBuildFileMetadata(file, "IconMarkup");
                    if (string.IsNullOrWhiteSpace(iconMarkup) == false)
                    {
                        BuildJsonToMarkup(context, file, iconJson, ns, iconMarkup);
                    }
                }
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

        private static void BuildJsonToClass(GeneratorExecutionContext context, AdditionalText file, IconJson iconJson, string ns, string entityName)
        {
            var className = entityName.AsName();

            Build(context, file, iconJson, className, (builder, json) => 
            {
                AppendHeader(builder, json);

                builder.AppendLine($"using System;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ns};");
                builder.AppendLine();
                builder.AppendLine($"public class {className}{Environment.NewLine}{{");

                foreach (var glyph in json.glyphs)
                {
                    builder.AppendLine($"    /// <summary>");
                    builder.AppendLine($"    /// [{glyph.icon_id}] {glyph.font_class}");
                    builder.AppendLine($"    /// </summary>");
                    builder.AppendLine($"    public const string {glyph.name.AsName()} = \"\\x{glyph.unicode}\";");
                }

                builder.AppendLine($"}}");
            });
        }

        private static void BuildJsonToMarkup(GeneratorExecutionContext context, AdditionalText file, IconJson iconJson, string ns, string entityName)
        {
            entityName = entityName.AsName();

            var markupKeysName = $"{entityName}Keys";
            var markupName = $"{entityName}Extension";

            Build(context, file, iconJson, markupKeysName, (builder, json) =>
            {
                AppendHeader(builder, json);

                builder.AppendLine($"using System;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ns};");
                builder.AppendLine();
                builder.AppendLine($"public enum {markupKeysName}{Environment.NewLine}{{");

                foreach (var glyph in json.glyphs)
                {
                    builder.AppendLine($"    {glyph.name.AsName()},");
                }

                builder.AppendLine($"}}");
            });

            Build(context, file, iconJson, markupName, (builder, json) =>
            {
                AppendHeader(builder, json);

                builder.AppendLine($"using System;");
                builder.AppendLine($"using System.Collections.Generic;");
                builder.AppendLine($"using Avalonia.Markup.Xaml;");
                builder.AppendLine();
                builder.AppendLine($"namespace {ns};");
                builder.AppendLine();
                builder.AppendLine($"public class {markupName} : MarkupExtension");
                builder.AppendLine($"{{");
                builder.AppendLine($"    private static readonly Dictionary<{markupKeysName}, string> Values = new Dictionary<{markupKeysName}, string>()");
                builder.AppendLine($"    {{");

                foreach (var glyph in json.glyphs)
                {
                    builder.AppendLine($"        {{ {markupKeysName}.{glyph.name.AsName()}, \"\\x{glyph.unicode}\" }},");
                }

                builder.AppendLine($"    }};");
                builder.AppendLine();
                builder.AppendLine($"    private readonly {markupKeysName} _key;");
                builder.AppendLine();
                builder.AppendLine($"    public {markupName}({markupKeysName} key) => _key = key;");
                builder.AppendLine();
                builder.AppendLine(@"
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Values.TryGetValue(_key, out var v))
        {
            return v;
        }

        return _key;
    }
}");
            });
        }

        private static void Build(GeneratorExecutionContext context, AdditionalText file, IconJson iconJson, string entityName, Action<StringBuilder,IconJson> contentBuilder)
        {
            try
            {
                var builder = new StringBuilder();
                contentBuilder(builder, iconJson);
                context.AddSource($"{entityName}.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
            catch (Exception e)
            {
                context.ReportError("NSIG002", "Invalid Icon Json File", $"{e.Message}", "", "", Location.Create(file.Path, new TextSpan(), new LinePositionSpan()));
            }
        }

        private static void AppendHeader(StringBuilder builder, IconJson json)
        {
            builder.AppendLine($"// This file is generated by Nlnet Iconfont Generator.");
            builder.AppendLine($"// Id:          {json.id}");
            builder.AppendLine($"// Name:        {json.name}");
            builder.AppendLine($"// FontFamily:  {json.font_family}");
            builder.AppendLine($"// Description: {json.description}");
            builder.AppendLine($"// Count:       {json.glyphs.Count}");
            builder.AppendLine();
        }
    }
}
