using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Models;
using File = MCFire.Modules.Files.Models.File;

namespace MCFire.Modules.Files.Services
{
    [Export(typeof(IFormat))]
    public class Format : IFormat<File>
    {
        #region Fields

        readonly List<string> _extensions = new List<string>();
        readonly IEventAggregator _aggregator;
        readonly object _lock = new object();

        #endregion

        #region Constructor

        [ImportingConstructor]
        public Format(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
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
            var file = new File(parent, info);
            _aggregator.Publish(new FileCreatedEvent<File>(file));
            return file;
        }

        public virtual bool TryAddExtension(string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            lock (_lock)
            {
                if (_extensions.Contains(extension))
                    return false;
                _extensions.Add(extension);
            }
            _aggregator.Publish(new FormatExtensionsChangedEvent<File>(this, extension, null));
            return true;
        }

        public virtual bool TryRemoveExtension(string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            lock (_lock)
            {
                if (!_extensions.Remove(extension)) return false;
            }

            _aggregator.Publish(new FormatExtensionsChangedEvent<File>(this, null, extension));
            return true;
        }

        #endregion

        #region Properties

        public IEnumerable<string> Extensions
        {
            get
            {
                lock (_lock)
                {
                    return new List<string>(_extensions);
                }
            }
        }

        public virtual IEnumerable<string> DefaultExtensions
        {
            get { yield break; }
        }

        #endregion
    }
}
