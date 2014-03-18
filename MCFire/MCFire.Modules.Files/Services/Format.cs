using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using MCFire.Modules.Infrastructure;
using File = MCFire.Modules.Files.Models.File;

namespace MCFire.Modules.Files.Services
{
    [Export]
    public class Format : IFormat<File>
    {
        #region Fields

        public event EventHandler FileCreated;
        public event EventHandler FileOpened;
        public event EventHandler FileClosing;
        public event EventHandler FileClosed;
        public event EventHandler ExtensionsChanged;

        readonly List<string> _extensions = new List<string>();

        #endregion

        #region Constructor

        public Format()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            _extensions = DefaultExtensions.ToList();
        }

        #endregion

        #region Methods

        public IFile CreateFile(IFolder parent, FileInfo info)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (info == null) throw new ArgumentNullException("info");

            if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("parent.Path must equal info.DirectoryInfo");

            return ConstructFile(parent, info);
        }

        File IFormat<File>.CreateFile(IFolder parent, FileInfo info)
        {
            return CreateFile(parent, info) as File;
        }

        protected virtual File ConstructFile(IFolder parent, FileInfo info)
        {
            return new File(parent, info);
        }

        public bool TryAddExtension(string extension)
        {
            if (_extensions.Contains(extension))
                return false;

            _extensions.Add(extension);
            OnExtensionsChanged();
            return true;
        }

        public bool TryRemoveExtension(string extension)
        {
            if (!_extensions.Remove(extension)) return false;

            OnExtensionsChanged();
            return true;
        }

        public virtual void RegisterChildFormat(IFormat format)
        {
            format.FileCreated += (s, e) => OnFileCreated();
            format.FileOpened += (s, e) => OnFileOpened();
            format.FileClosing += (s, e) => OnFileClosing();
            format.FileClosed += (s, e) => OnFileClosed();

            // remove any extensions that are also held by child format
            foreach (var extension in _extensions.Where(extension => format.Extensions.Contains(extension)))
            {
                TryRemoveExtension(extension);
            }
        }

        #region Event Invocators

        protected virtual void OnFileCreated()
        {
            var handler = FileCreated;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnFileOpened()
        {
            var handler = FileOpened;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnFileClosing()
        {
            var handler = FileClosing;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnFileClosed()
        {
            var handler = FileClosed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnExtensionsChanged()
        {
            var handler = ExtensionsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Properties

        public IEnumerable<string> Extensions
        {
            get { return _extensions; }
        }

        public virtual IEnumerable<string> DefaultExtensions
        {
            get { yield break; }
        }

        #endregion
    }
}
