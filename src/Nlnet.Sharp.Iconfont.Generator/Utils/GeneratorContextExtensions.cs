using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;

namespace Nlnet.Sharp.Iconfont.Generator
{
    internal static class GeneratorContextExtensions
    {
        #region MsBuild Property

         public static string GetMsBuildProperty(
            this GeneratorExecutionContext context,
            string name,
            string defaultValue = "")
        {
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out var value);
            return value ?? defaultValue;
        }

        public static string GetMsBuildFileMetadata(
            this GeneratorExecutionContext context,
            AdditionalText file,
            string name,
            string defaultValue = "")
        {
            context.AnalyzerConfigOptions.GetOptions(file).TryGetValue($"build_metadata.AdditionalFiles.{name}", out var value);
            return value ?? defaultValue;
        }

        public static string[] GetStringArrayProperty(this GeneratorExecutionContext context, string name)
        {
            var value = context.GetMsBuildProperty(name);
            return value.Contains(";") ? value.Split(';') : new[] { value };
        }

        public static TEnum GetEnumProperty<TEnum>(this GeneratorExecutionContext context, string name) where TEnum : struct
        {
            var value = context.GetMsBuildProperty(name);
            return Enum.TryParse(value, true, out TEnum behavior) ? behavior : default;
        }

        public static bool GetBoolProperty(this GeneratorExecutionContext context, string name)
        {
            var value = context.GetMsBuildProperty(name);
            return bool.TryParse(value, out var result) ? result : default;
        }
        
        public static string[] GetStringArrayProperty(this GeneratorExecutionContext context, AdditionalText file,string name)
        {
            var value = context.GetMsBuildFileMetadata(file, name);
            return value.Contains(";") ? value.Split(';') : new[] { value };
        }

        public static TEnum GetEnumProperty<TEnum>(this GeneratorExecutionContext context, AdditionalText file,string name) where TEnum : struct
        {
            var value = context.GetMsBuildFileMetadata(file, name);
            return Enum.TryParse(value, true, out TEnum behavior) ? behavior : default;
        }

        public static bool GetBoolProperty(this GeneratorExecutionContext context, AdditionalText file, string name)
        {
            var value = context.GetMsBuildFileMetadata(file, name);
            return bool.TryParse(value, out var result) ? result : default;
        }

        public static string GetDefaultNamespace(this GeneratorExecutionContext context)
        {
            return GetMsBuildProperty(context, "RootNamespace");
        }

        public static void OptionsDiagnostic(this GeneratorExecutionContext context)
        {
            Trace.WriteLine($"+++++++++++++++++++++++++++++ ");
            foreach (var option in context.AnalyzerConfigOptions.GlobalOptions.Keys)
            {
                Trace.WriteLine($"+++++++++++ {option}");
            }
        }

        #endregion



        #region Diagnostic Report

          public static void ReportInfo(
            this GeneratorExecutionContext context,
            string id,
            string title,
            string message = null,
            string description = null, 
            string helpLinkUri = null, 
            Location location = null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: message ?? title,
                        category: "Usage",
                        defaultSeverity: DiagnosticSeverity.Info,
                        isEnabledByDefault: true,
                        description: description,
                        helpLinkUri: helpLinkUri),
                    location ?? Location.None));
        }

        public static void ReportWarning(
            this GeneratorExecutionContext context,
            string id,
            string title,
            string message = null,
            string description = null,
            string helpLinkUri = null,
            Location location = null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: message ?? title,
                        category: "Usage",
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: description,
                        helpLinkUri: helpLinkUri),
                    location ?? Location.None));
        }

        public static void ReportError(
            this GeneratorExecutionContext context,
            string id,
            string title,
            string message = null,
            string description = null,
            string helpLinkUri = null,
            Location location = null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: id,
                        title: title,
                        messageFormat: message ?? title,
                        category: "Usage",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true,
                        description: description,
                        helpLinkUri: helpLinkUri),
                    location ?? Location.None));
        }

        #endregion
    }
}
