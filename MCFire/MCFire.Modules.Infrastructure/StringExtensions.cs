using System;
using System.Text;

namespace MCFire.Modules.Infrastructure
{
    public static class StringExtensions
    {
        public static string FindReplaceLastOccurance(this string source, string find, string replace)
        {
            var sb = new StringBuilder(source);
            sb.Replace(find, replace, sb.ToString().LastIndexOf(find, StringComparison.Ordinal), find.Length);
            return sb.ToString();
        } 
    }
}
