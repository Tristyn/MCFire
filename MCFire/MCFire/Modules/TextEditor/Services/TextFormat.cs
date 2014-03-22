using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Services
{
    [Export(typeof(IFormat))]
    [Export(typeof(IFormat<TextFile>))]
    class TextFormat : Format, IFormat<TextFile>
    {
        #region Constructor

        [ImportingConstructor]
        public TextFormat(IEventAggregator aggregator) : base(aggregator){}

        #endregion

        #region Methods

        IFile IFormat.CreateFile(IFolder parent, FileInfo info)
        {
            return CreateFile(parent, info);
        }

        public new TextFile CreateFile(IFolder parent, FileInfo info)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (info == null) throw new ArgumentNullException("info");

            if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("parent.Path must equal info.DirectoryInfo");

            var file = new TextFile(parent, info);
            Aggregator.Publish(new FileCreatedMessage<TextFile>(file));
            return file;
        }

        #endregion

        #region Properties

        IEnumerable<string> IFormat.DefaultExtensions { get { yield return ".txt"; } }

        #endregion
    }
}
