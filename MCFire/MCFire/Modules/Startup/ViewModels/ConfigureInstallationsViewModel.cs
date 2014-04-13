using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure.Interfaces;
using MCFire.Modules.WorldExplorer.Models;
using MCFire.Modules.WorldExplorer.Services;
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
        Installation _install;
        IEnumerable<WorldState> _worlds;
        Timer _refreshTimer;

        bool? _minecraftExists;
        bool? _minecraftUnknown;
        bool _loading = true;
        int _worldCount;

        public void ContinueNoInstall()
        {
            var result = MessageBox.Show(Application.Current.MainWindow,
                "No Minecraft installations have been set, it is critical that an installation is set. You can set installations later. Are you sure you want to continue? ",
                "No Minecraft installation has been found.", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Close();
        }

        public void Close()
        {
            if (CloseOverlay != null) CloseOverlay(this, EventArgs.Empty);
        }

        public void Continue()
        {
            Close(); // TODO:
        }

        public void DownloadMinecraft()
        {
            Process.Start("http://minecraft.net");
        }

        public void FindMainInstall()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select a folder to be added to the File Explorer.",
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var install = Installation.New(dialog.SelectedPath);
            if (install == null)
            {
                MessageBox.Show("MC Fire did not detect that the specified folder was a minecraft installation.",
                    "Not a Minecraft installation");
                return;
            }

            _explorerService.Installations.Add(install);
            Install = install;
        }

        public void UseSampleMap()
        {
            throw new NotImplementedException();
        }

        public async void Loaded()
        {
            await Task.Delay(1500);

            // if minecraft exists on disk, add it to explorer service and display it
            var newInstall = GetMainInstallation();
            if (newInstall != null)
            {
                // add install to explorer service if it wasn't there already
                if (!_explorerService.Installations.Contains(newInstall))
                    _explorerService.Installations.Add(newInstall);
                Install = newInstall;
                return;
            }

            // minecraft not known, setting Install to null will make the ui ask for installations
            Install = null;
        }

        [CanBeNull]
        Installation GetMainInstallation()
        {
            var minecraftPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")
                    .ToLower();

            // if .minecraft has already been added
            var minecraftInstall =
                _explorerService.Installations.Where(install => install != null).FirstOrDefault(install => install.Path.ToLower() == minecraftPath);
            if (minecraftInstall != null)
            {
                return minecraftInstall;
            }

            // if minecraft exists on disk, add it to explorer service and display it
            var newInstall = Installation.New(minecraftPath);
            return newInstall; // null if it doesn't exist/hasn't been initialized
        }

        [CanBeNull]
        public Installation Install
        {
            get { return _install; }
            private set
            {
                _install = value;
                MinecraftExists = value != null ? (bool?)true : null;
                MinecraftUnknown = value == null ? (bool?)true : null;
                if (value != null)
                    Worlds = from world in value.Worlds
                             select new WorldState(world.Level.GameType, world.Level.LevelName);
                Loading = false;
                NotifyOfPropertyChange(() => Install);
                NotifyOfPropertyChange(() => CanClose);
            }
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
                    if(_refreshTimer==null)return;

                    // check for installation
                    var install = GetMainInstallation();
                    if (install == null) return;

                    // install exists
                    _refreshTimer.Stop();
                    _refreshTimer.Dispose();
                    _refreshTimer = null;

                    // add install to explorer service if it wasn't there already
                    if (!_explorerService.Installations.Contains(install))
                        _explorerService.Installations.Add(install);
                    Install = install;
                };
                _refreshTimer.Interval = 5000;
                _refreshTimer.Start();
            }
        }

        public bool CanContinue
        {
            get { return Install != null; }
        }

        public bool CanClose
        {
            get { return Install != null; }
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

        [CanBeNull]
        public IEnumerable<WorldState> Worlds
        {
            get { return _worlds; }
            private set
            {
                if (Equals(value, _worlds)) return;
                _worlds = value;

                WorldCount = value != null ? value.Count() : 0;
                NotifyOfPropertyChange(() => Worlds);
            }
        }

        public int WorldCount
        {
            get { return _worldCount; }
            private set
            {
                if (value == _worldCount) return;
                _worldCount = value;
                NotifyOfPropertyChange(() => WorldCount);
            }
        }

        public event EventHandler CloseOverlay;
    }

    public class WorldState
    {
        public WorldState(GameType gameType, string title)
        {
            GameType = gameType;
            Title = title;
        }

        public string Title { get; private set; }
        public GameType GameType { get; private set; }
    }
}
