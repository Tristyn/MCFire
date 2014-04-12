using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using MCFire.Modules.Infrastructure.Interfaces;
using MCFire.Modules.WorldExplorer.Models;
using MCFire.Modules.WorldExplorer.Services;
using Substrate;

namespace MCFire.Modules.Startup.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ConfigureInstallationsViewModel : PropertyChangedBase, IModalOverlay
    {
        [Import]
        WorldExplorerService _explorerService;
        bool? _minecraftExists;
        Installation _install;
        private IEnumerable<WorldState> _worlds;
        private bool _minecraftUnknown;
        private bool _loading = true;
        private int _worldCount;

        public void Close()
        {
            if (CloseOverlay != null) CloseOverlay(this, EventArgs.Empty);
        }

        public void Continue()
        {
            Close(); // TODO:
        }

        public async void Loaded()
        {
            await Task.Delay(1500);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft").ToLower();

            // if .minecraft has already been added
            var minecraftInstall = _explorerService.Installations.FirstOrDefault(install => install.Path.ToLower() == path);
            if (minecraftInstall != null)
            {
                Install = minecraftInstall;
                return;
            }

            // if minecraft exists on disk.
            var newInstall = Installation.New(path);
            if (newInstall != null)
            {
                _explorerService.Installations.Add(newInstall);
                Install = newInstall;
            }

            MinecraftExists = false;
        }

        public Installation Install
        {
            get { return _install; }
            private set
            {
                _install = value;
                MinecraftExists = _install != null;
                MinecraftUnknown = _install == null;
                Worlds = from world in value.Worlds
                         select new WorldState(world.Level.GameType, world.Level.LevelName);
                Loading = false;
                NotifyOfPropertyChange(() => Install);
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

        public bool MinecraftUnknown
        {
            get { return _minecraftUnknown; }
            private set
            {
                if (value.Equals(_minecraftUnknown)) return;
                _minecraftUnknown = value;
                NotifyOfPropertyChange(() => MinecraftUnknown);
            }
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

        public IEnumerable<WorldState> Worlds
        {
            get { return _worlds; }
            private set
            {
                if (Equals(value, _worlds)) return;
                _worlds = value;
                WorldCount = _worlds.Count();
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
