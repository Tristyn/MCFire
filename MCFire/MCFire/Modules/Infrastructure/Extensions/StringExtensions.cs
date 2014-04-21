using System;
using System.IO;
using System.Text;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string FindReplaceLastOccurance(this string source, string find, string replace)
        {
            var sb = new StringBuilder(source);
            sb.Replace(find, replace, sb.ToString().LastIndexOf(find, StringComparison.Ordinal), find.Length);
            return sb.ToString();
        }

        public static string NormalizePath(this string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToLowerInvariant();
        }
    }
}
