using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.TextEditor.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class GameInstallation : Installation
    {
        public GameInstallation(IFolder folder)
            : base(folder)
        {
            var savesFolder = folder.Folders.FirstOrDefault(f => f.Name.ToLower() == "saves");
            if (savesFolder == null)
            {
                // when saves does pop up, call LinkWorlds via this
                ListenForSaves(folder);
                Children = new ObservableCollection<WorldBrowserItem>();
            }
            else LinkWorlds(savesFolder);
        }

        void ListenForSaves(IFolder folder)
        {
            folder.Folders.CollectionChanged += CheckInstallationforSavesFolder;
        }

        void CheckInstallationforSavesFolder(object s, NotifyCollectionChangedEventArgs e)
        {
            var folders = e.NewItems.Cast<IFolder>();
            var savesFolder = folders.FirstOrDefault(item => item.Name.ToLower() == "saves");
            if (savesFolder == null) return;

            Folder.Folders.CollectionChanged -= CheckInstallationforSavesFolder;
            LinkWorlds(savesFolder);
        }

        /// <summary>
        /// Sets Worlds to react to the content of savesFolder
        /// </summary>
        /// <param name="savesFolder"></param>
        void LinkWorlds(IFolder savesFolder)
        {
            Worlds = savesFolder.Folders.Link
                        (fold => new World(fold),
                        (fold, world) => world.Folder == fold);
            if (Children == null)
                Children = Worlds.Link<WorldBrowserItem, World>();
            else
                Worlds.LinkExisting(Children);
        }

        public override string Title
        {
            get { return Folder.Name.ToLower() == ".minecraft" ? "Mine Craft" : Folder.Name; }
        }

        public override InstallationType Type
        {
            get { return InstallationType.Game; }
        }

        public override sealed ObservableCollection<WorldBrowserItem> Children { get; protected set; }
        public override sealed ObservableCollection<World> Worlds { get; protected set; }

        public TextContent Options
        {
            get
            {
                var optionsFile = Folder.Files.FirstOrDefault(file => file.Name.ToLower() == "options.txt");
                if (optionsFile == null) return null;

                TextContent content;
                return optionsFile.TryOpenContent(out content) ? content : null;
            }
        }
    }
}
