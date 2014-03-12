using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Security;
using System.Windows;
using Caliburn.Micro;
using Gemini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gemini.Framework.Services;

namespace MCFire
{
    class MCFireBootstrapper : AppBootstrapper
    {
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            // get the application assembly, and the Metro assembly.
            var assemblies = new List<Assembly>(base.SelectAssemblies())
            {
                typeof (Gemini.Modules.Metro.ViewModels.MainWindowViewModel).Assembly
            };

            // get assemblies in mods folder
            try
            {
                var modsPath = Path.GetFullPath(@"./.MODS/");
                //create mods folder if it doesnt exist, then walk its subdirectories
                Directory.CreateDirectory(modsPath);
                foreach (var directoryInfo in WalkDirectoryTree(new DirectoryInfo(modsPath)))
                {
                    var catalog = new DirectoryCatalog(directoryInfo.FullName);

                    // hack out the mods assemblies with this horrible code.
                    assemblies.AddRange(catalog.Parts
                        .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                        .Where(assembly => !assemblies.Contains(assembly)));
                }
            }
            catch (SecurityException) { Console.WriteLine("Could not create mods folder. Security exception."); }
            catch (PathTooLongException) { Console.WriteLine("Could not create mods folder. Path too long."); }
            catch (UnauthorizedAccessException) { Console.WriteLine("Could not create mods folder. Unauthorized access."); }
            catch (DirectoryNotFoundException) { Console.WriteLine("Could not create mods folder. Directory unavailable."); }

            // reverse assemblies, so mods are prioritized.
            assemblies.Reverse();
            return assemblies;
        }

        /// <summary>
        /// returns all subdirectories in the root directory.
        /// root directory is included.
        /// </summary>
        /// <param name="root">The directory to begin the search.</param>
        private static IEnumerable<DirectoryInfo> WalkDirectoryTree(DirectoryInfo root)
        {
            var directories = new List<DirectoryInfo> { root };

            try
            {
                var subDirs = root.GetDirectories();

                foreach (var dirInfo in subDirs)
                {
                    // yes i know this is quadratic. it works

                    directories.AddRange(WalkDirectoryTree(dirInfo));
                }
            }
            catch (DirectoryNotFoundException) { }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }

            return directories;
        }
    }
}
