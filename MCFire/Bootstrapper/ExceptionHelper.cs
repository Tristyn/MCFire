using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace MCFire.Bootstrapper
{
    public static class ExceptionHelper
    {
        public static void UnhandledUiException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorMessage = string.Format("An application error occurred. We recommend that you save your work and restart the application. \n\nDo you want to continue?\n(if you click Yes you will continue with your work, if you click No the application will close)");
            if (MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.No)
            {
                Application.Current.Shutdown();
            }

            // log it
            var exceptionType = WriteExceptionDetails(e.Exception);
            var date = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            var logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase),
                String.Format(@"Exception {0} {1}.txt", date, e.Exception.GetType()));
            logPath = Path.GetInvalidPathChars().Aggregate(logPath, (current, c) => current.Replace(c.ToString(), string.Empty));
            File.WriteAllText(new Uri(logPath).LocalPath, exceptionType);

            Process.Start(logPath);
        }

        public static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var errorMessage = string.Format("An unrecoverable error has occurred. We are sorry.");
            MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // log it
            var exceptionType = ExceptionHelper.WriteExceptionDetails(e.ExceptionObject as Exception);
            var date = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            var logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase),
                String.Format(@"Exception {0} {1}.txt", date, e.GetType()));
            logPath = Path.GetInvalidPathChars()
                .Aggregate(logPath, (current, c) => current.Replace(c.ToString(), string.Empty));
            File.WriteAllText(new Uri(logPath).LocalPath, exceptionType);

            Process.Start(logPath);
        }

        public static string WriteExceptionDetails(Exception exception)
        {
            var builder = new StringBuilder(600);
            WriteExceptionDetails(exception, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Converts an exception to a user friendly string
        /// </summary>
        /// <param name="exception">The exception to convert to a string</param>
        /// <param name="builderToFill">The StringBuilder that the message will be written to</param>
        /// <param name="level">The amount of indentation characters to use</param>
        public static void WriteExceptionDetails(Exception exception, StringBuilder builderToFill, int level = 0)
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
