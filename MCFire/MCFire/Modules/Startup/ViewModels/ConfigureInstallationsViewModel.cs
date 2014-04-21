using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Explorer.Services;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Interfaces;
using MCFire.Modules.Infrastructure.Models;
using MCFire.Properties;
using NUnrar.Archive;
using NUnrar.Common;
using Substrate;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace MCFire.Modules.Startup.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ConfigureInstallationsViewModel : PropertyChangedBase, IModalOverlay
    {
        [Import]
        WorldExplorerService _explorerService;

        BindableCollection<Installation> _installs;
        Installation _install;
        IEnumerable<WorldState> _worlds;
        Timer _refreshTimer;

        bool? _minecraftExists;
        bool? _minecraftUnknown;
        bool _loading = true;
        int _worldCount;
        private string _sampleMapMessage;

        public ConfigureInstallationsViewModel()
        {
            SampleMapMessage = "Use Sample Map";
        }

        public void ContinueNoInstall()
        {
            var result = MessageBox.Show(Application.Current.MainWindow,
                "No Minecraft installations have been set, it is critical that an installation is set. You can set installations later. Are you sure you want to continue? ",
                "No Minecraft installation has been found.", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Continue();
        }

        public void Continue()
        {
            if (CloseOverlay != null) CloseOverlay(this, EventArgs.Empty);
        }

        public void DownloadMinecraft()
        {
            Process.Start("http://minecraft.net");
        }

        /// <summary>
        /// Opens a FolderBrowserDialog for the user to select an installation.
        /// Message boxes and returns null if no installation was found.
        /// </summary>
        /// <returns>An installation. Can be null.</returns>
        [CanBeNull]
        static Installation BrowseForInstallation()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select the folder that Minecraft is installed in.",
                SelectedPath = MCFireDirectories.MinecraftAppdata
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return null;

            var install = Installation.New(dialog.SelectedPath);
            if (install == null)
                MessageBox.Show("MC Fire did not detect that the specified folder was a minecraft installation.",
                    "Not a Minecraft installation");
            return install;
        }

        public void UseSampleMap()
        {
            try
            {
                var rarUri = Path.Combine(MCFireDirectories.Install, "Resources", "SampleWorld.rar");
                var rarDir = new Uri(rarUri).LocalPath;
                var extractDir = Path.Combine(MCFireDirectories.MCFireAppdata, "SampleWorld");

                Directory.Delete(extractDir, true);
                RarArchive.WriteToDirectory(rarDir, MCFireDirectories.MCFireAppdata, ExtractOptions.ExtractFullPath);
                _explorerService.TryAddInstallation(Installation.New(extractDir));
            }
            catch (Exception ex)
            {
                SampleMapMessage = "Failed extracting sample map";
            }
        }

        public void AddGame()
        {
            var install = BrowseForInstallation();
            if (install == null) return;

            if (!(install is GameInstallation))
                MessageBox.Show("MC Fire detected that the specified folder was a server installation.");

            _explorerService.TryAddInstallation(install);
        }

        public void AddServer()
        {
            var install = BrowseForInstallation();
            if (install == null) return;

            if (!(install is ServerInstallation))
                MessageBox.Show("MC Fire detected that the specified folder was a game installation.");

            _explorerService.TryAddInstallation(install);
        }

        public void RemoveInstall(object dataContext)
        {
            var install = dataContext as Installation;
            Debug.Assert(install != null);
            if (Installs.Count == 1)
            {
                var result = MessageBox.Show("Are you sure you want to remove the only installation from MC Fire?", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;
            }

            if (install.Path.NormalizePath() == MCFireDirectories.MinecraftAppdata)
                Settings.Default.DontAddDefaultInstall = true;
            _explorerService.Installations.Remove(install);
        }

        public async void Loaded()
        {
            await Task.Delay(1500);

            // if minecraft exists on disk, add it to explorer service and display it
            var newInstall = GetAnyInstallation();
            if (newInstall == null) return;

            // add install to explorer service if it wasn't there already
            _explorerService.TryAddInstallation(newInstall);

            Installs = _explorerService.Installations.Link<Installation, Installation, BindableCollection<Installation>>();
        }

        [CanBeNull]
        Installation GetAnyInstallation()
        {
            // if an install has already been added
            var minecraftInstall = _explorerService.Installations.FirstOrDefault();
            if (minecraftInstall != null)
            {
                return minecraftInstall;
            }

            // if minecraft exists on disk, add it to explorer service and display it
            if (Settings.Default.DontAddDefaultInstall) return null;
            var newInstall = Installation.New(MCFireDirectories.MinecraftAppdata);
            return newInstall; // null if it doesn't exist/hasn't been initialized
        }

        public BindableCollection<Installation> Installs
        {
            get { return _installs; }
            set
            {
                if (value == _installs) return;
                if (value != null) Loading = false;

                if (_installs != null)
                    _installs.CollectionChanged -= CheckAnyInstalls;
                _installs = value;
                if (value != null)
                    value.CollectionChanged += CheckAnyInstalls;
                CheckAnyInstalls(null, null);

                NotifyOfPropertyChange(() => Installs);
            }
        }

        void CheckAnyInstalls(object s, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => CanContinue);
            if (Installs == null || !Installs.Any())
            {
                MinecraftExists = false;
                MinecraftUnknown = true;
                return;
            }

            MinecraftExists = true;
            MinecraftUnknown = false;
        }

        public bool? MinecraftExists
        {
            get { return _minecraftExists; }
            private set
            {
                if (value.Equals(_minecraftExists)) return;
                _minecraftExists = value;
                NotifyOfPropertyChange(() => MinecraftExists);
            }
        }

        public bool? MinecraftUnknown
        {
            get { return _minecraftUnknown; }
            private set
            {
                if (value.Equals(_minecraftUnknown)) return;
                _minecraftUnknown = value;
                NotifyOfPropertyChange(() => MinecraftUnknown);

                // if true, recheck every 5 seconds
                if (_refreshTimer != null) return;
                _refreshTimer = new Timer();
                _refreshTimer.Tick += (s, e) =>
                {
                    // ghost delegate after disposing of the timer
                    if (_refreshTimer == null) return;

                    // check for installation
                    var install = GetAnyInstallation();
                    if (install == null) return;

                    // install exists
                    _refreshTimer.Stop();
                    _refreshTimer.Dispose();
                    _refreshTimer = null;

                    // add install to explorer service if it wasn't there already
                    _explorerService.TryAddInstallation(install);
                };
                _refreshTimer.Interval = 5000;
                _refreshTimer.Start();
            }
        }

        public bool CanContinue
        {
            get { return Installs != null && Installs.Any(); }
        }

        public bool Loading
        {
            get { return _loading; }
            private set
            {
                if (value.Equals(_loading)) return;
                _loading = value;
                NotifyOfPropertyChange(() => Loading);
            }
        }

        public string SampleMapMessage
        {
            get { return _sampleMapMessage; }
            private set
            {
                if (value == _sampleMapMessage) return;
                _sampleMapMessage = value;
                NotifyOfPropertyChange(() => SampleMapMessage);
            }
        }

        public event EventHandler CloseOverlay;
    }


    public class WorldState
    {
        int _worldCount;

        public WorldState(GameType gameType, string title)
        {
            GameType = gameType;
            Title = title;
        }

        public string Title { get; private set; }
        public GameType GameType { get; private set; }
    }
}
