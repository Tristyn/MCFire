using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Security;
using System.Windows;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gemini.Framework.Services;

namespace MCFire
{
    /// <summary>
    /// Copied from gemini libraries because TGJONES WONT MERGE PULL REQUEST #34 COME ON WHAT GIVES ITS ONE WORD
    /// </summary>
    class MCFireBootstrapper : Bootstrapper<IMainWindow>
    {
        protected CompositionContainer Container;

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            if(Execute.InDesignMode) return new List<Assembly>();

            // get the application assembly, and the Metro assembly.
            var assemblies = new List<Assembly>();
            // get assemblies in mods folder
            try
            {
                var modsPath = Path.GetFullPath(@"./.MODS/");
                //create mods folder if it doesnt exist, then walk its subdirectories
                Directory.CreateDirectory(modsPath);
                foreach (var directoryInfo in WalkDirectoryTree(new DirectoryInfo(modsPath)))
                {
                    var catalog = new DirectoryCatalog(directoryInfo.FullName);

                    /*
                    ░░░░░▄▄▄▄▀▀▀▀▀▀▀▀▄▄▄▄▄▄░░░░░░░
                    ░░░░░█░░░░▒▒▒▒▒▒▒▒▒▒▒▒░░▀▀▄░░░░
                    ░░░░█░░░▒▒▒▒▒▒░░░░░░░░▒▒▒░░█░░░
                    ░░░█░░░░░░▄██▀▄▄░░░░░▄▄▄░░░░█░░
                    ░▄▀▒▄▄▄▒░█▀▀▀▀▄▄█░░░██▄▄█░░░░█░
                    █░▒█▒▄░▀▄▄▄▀░░░░░░░░█░░░▒▒▒▒▒░█
                    █░▒█░█▀▄▄░░░░░█▀░░░░▀▄░░▄▀▀▀▄▒█
                    ░█░▀▄░█▄░█▀▄▄░▀░▀▀░▄▄▀░░░░█░░█░
                    ░░█░░░▀▄▀█▄▄░█▀▀▀▄▄▄▄▀▀█▀██░█░░
                    ░░░█░░░░██░░▀█▄▄▄█▄▄█▄████░█░░░
                    ░░░░█░░░░▀▀▄░█░░░█░█▀██████░█░░
                    ░░░░░▀▄░░░░░▀▀▄▄▄█▄█▄█▄█▄▀░░█░░
                    ░░░░░░░▀▄▄░▒▒▒▒░░░░░░░░░░▒░░░█░
                    ░░░░░░░░░░▀▀▄▄░▒▒▒▒▒▒▒▒▒▒░░░░█░
                    ░░░░░░░░░░░░░░▀▄▄▄▄▄░░░░░░░░█░░
                    */

                    // hack out the mods assemblies with this horrible code.
                    assemblies.AddRange(catalog.Parts
                        .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                        .Where(assembly => !assemblies.Contains(assembly)));
                }
            }
            catch (SecurityException) { }
            catch (PathTooLongException) { }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }

            // add priority MCFire assemblies.
            assemblies.AddRange(base.SelectAssemblies());
            return assemblies;
        }

        /// <summary>
        /// Returns all subdirectories in the root directory.
        /// Root directory is included.
        /// The folders are organized in alphabetical order, with child folders prioritized.
        /// </summary>
        /// <param name="root">The directory to begin the search.</param>
        private static IEnumerable<DirectoryInfo> WalkDirectoryTree(DirectoryInfo root)
        {
            var directories = new List<DirectoryInfo>();

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

            directories.Add(root);

            return directories;
        }

        #region Gemini AppBootstrapper

        /// <summary>
        /// By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            if (Execute.InDesignMode) return;

            var ignoredAssembies = new[]
            {
                "Assimp32.dll",
                "AssimpNet.dll"
            };

            // Add all assemblies to AssemblySource, excluding assimp.dll because i dont like it specifically
            var rootDir = new DirectoryInfo(@"./");
            foreach (var fileInfo in rootDir.GetFiles("*.dll").Where(info => !ignoredAssembies.Contains(info.Name)))
            {
                try
                {
                    var assemblyCatalog=new AssemblyCatalog(Assembly.LoadFrom(fileInfo.Name));
                    var parts = assemblyCatalog.Parts
                        .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly).Where(assembly => !AssemblySource.Instance.Contains(assembly));
                    AssemblySource.Instance.AddRange(parts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load {0}: {1}", fileInfo.Name, ex);
                    MessageBox.Show(ex.ToString());
                }
            }

            //var directoryCatalog = new DirectoryCatalog(@"./",);
            //AssemblySource.Instance.AddRange(
            //    directoryCatalog.Parts
            //        .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
            //        .Where(assembly => !AssemblySource.Instance.Contains(assembly)));


            // Prioritise the executable assembly. This allows the client project to override exports, including IShell.
            // The client project can override SelectAssemblies to choose which assemblies are prioritised.
            var priorityAssemblies = SelectAssemblies().ToList();
            var priorityCatalog = new AggregateCatalog(priorityAssemblies.Select(x => new AssemblyCatalog(x)));
            var priorityProvider = new CatalogExportProvider(priorityCatalog);

            // Now get all other assemblies (excluding the priority assemblies).
            var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !priorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));

            var mainProvider = new CatalogExportProvider(mainCatalog);
            Container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = Container;
            mainProvider.SourceProvider = Container;

            var batch = new CompositionBatch();

            BindServices(batch);
            batch.AddExportedValue(mainCatalog);

            Container.Compose(batch);
        }

        protected virtual void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(Container);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            Container.SatisfyImportsOnce(instance);
        }

        #endregion
    }
}
