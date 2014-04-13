using System;
using System.Collections;
using System.Text;

namespace MCFire.Bootstrapper
{
    public static class ExceptionHelper
    {
        public static string WriteExceptionDetails(Exception exception)
        {
            var builder = new StringBuilder(200);
            WriteExceptionDetails(exception,builder);
            return builder.ToString();
        }

        /// <summary>
        /// Converts an exception to a user friendly string
        /// </summary>
        /// <param name="exception">The exception to convert to a string</param>
        /// <param name="builderToFill">The StringBuilder that the message will be written to</param>
        /// <param name="level">The amount of indentation characters to use</param>
        public static void WriteExceptionDetails(Exception exception, StringBuilder builderToFill, int level=0)
        {
            var indent = new string(' ', level);

            if (level > 0)
            {
                builderToFill.AppendLine(indent + "=== INNER EXCEPTION ===");
            }

            Action<string> append = prop =>
            {
                var propInfo = exception.GetType().GetProperty(prop);
                var val = propInfo.GetValue(exception);

                if (val != null)
                {
                    builderToFill.AppendFormat("{0}{1}: {2}{3}", indent, prop, val.ToString(), Environment.NewLine);
                }
            };

            append("Message");
            append("HResult");
            append("HelpLink");
            append("Source");
            append("StackTrace");
            append("TargetSite");

            foreach (DictionaryEntry de in exception.Data)
            {
                builderToFill.AppendFormat("{0} {1} = {2}{3}", indent, de.Key, de.Value, Environment.NewLine);
            }

            if (exception.InnerException != null)
            {
                WriteExceptionDetails(exception.InnerException, builderToFill, ++level);
            }
        }
    }
}
