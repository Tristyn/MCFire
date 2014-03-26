using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Nbt.Models;

namespace MCFire.Modules.Nbt.Services
{
    [Export(typeof(IFormat))]
    [Export(typeof(IFormat<NbtFile>))]
    class NbtFormat : Format, IFormat<NbtFile>
    {
        [ImportingConstructor]
        public NbtFormat(IEventAggregator aggregator) : base(aggregator) { }

        IFile IFormat.CreateFile(IFolder parent, FileInfo info)
        {
            return CreateFile(parent, info);
        }

        public new NbtFile CreateFile(IFolder parent, FileInfo info)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (info == null) throw new ArgumentNullException("info");
            if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("parent.Path must equal info.DirectoryInfo");

            var file = new NbtFile(parent, info);
            Aggregator.Publish(new FileCreatedMessage<NbtFile>(file));

            return file;
        }

        IEnumerable<string> IFormat.DefaultExtensions
        {
            get { yield return ".dat"; }
        }
    }
}
