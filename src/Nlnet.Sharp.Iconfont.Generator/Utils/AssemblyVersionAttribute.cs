using System;
using System.Reflection;

namespace Nlnet.Sharp
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyVersionAttribute : Attribute
    {
        public string Version { get; set; }

        public AssemblyVersionAttribute(string version)
        {
            this.Version = version;
        }
    }

    public static class AssemblyVersionProvider
    {
        public static string GetVersion<T>()
        {
            return typeof(T).Assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version;
        }
    }
}