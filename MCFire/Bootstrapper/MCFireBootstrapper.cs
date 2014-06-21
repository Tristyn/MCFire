using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Threading;
using Caliburn.Micro;
using Gemini.Framework.Services;
using MCFire.Client;

namespace MCFire.Bootstrapper
{
    /// <summary>
    /// Copied from gemini libraries because TGJONES WONT MERGE PULL REQUEST #34 COME ON WHAT GIVES ITS ONE WORD
    /// </summary>
    sealed class MCFireBootstrapper : Bootstrapper<IMainWindow>
    {
        CompositionContainer _container;

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            if (Execute.InDesignMode) return new List<Assembly>();

            // get the application assembly, and the Metro assembly.
            var assemblies = new List<Assembly>();
            // get assemblies in mods folder
            try
            {
                var modsPath = MCFireDirectories.AppdataMods;
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
            assemblies.Add(typeof(Common.Library).Assembly);
            assemblies.Add(typeof(Core.Library).Assembly);
            assemblies.Add(typeof(Graphics.Library).Assembly);
            assemblies.Add(typeof(Client.Library).Assembly);
            assemblies.Add(typeof(Client.Gui.Library).Assembly);
            assemblies.Add(GetType().Assembly);
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

            // only output errors if in release
#if !DEBUG
            Dispatcher.CurrentDispatcher.UnhandledException += ExceptionHelper.UnhandledUiException;
            AppDomain.CurrentDomain.UnhandledException += ExceptionHelper.UnhandledException;
#endif
            var ignoredAssembies = new[]
            {
                "Assimp32.dll",
                "AssimpNet.dll"
            };

            // Add all assemblies to AssemblySource, excluding assimp.dll because i dont like it specifically
            var rootDir = new DirectoryInfo(@"./");
            var rootFiles = rootDir.GetFiles("*.dll").Where(info => !ignoredAssembies.Contains(info.Name)).ToList();
            foreach (var fileInfo in rootFiles)
            {
                try
                {
                    var assemblyCatalog = new AssemblyCatalog(Assembly.LoadFrom(fileInfo.Name));
                    var parts = assemblyCatalog.Parts
                        .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly).Where(assembly => !AssemblySource.Instance.Contains(assembly));
                    AssemblySource.Instance.AddRange(parts);
                }
                catch (BadImageFormatException ex)
                {
                }
            }


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
            _container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = _container;
            mainProvider.SourceProvider = _container;

            var batch = new CompositionBatch();

            BindServices(batch);
            batch.AddExportedValue(mainCatalog);

            _container.Compose(batch);
        }

        private void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            var export = exports.FirstOrDefault();
            if (export != null) return export;

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        #endregion
    }
}
