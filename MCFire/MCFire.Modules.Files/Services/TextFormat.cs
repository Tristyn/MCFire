using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Services
{
    [Export]
    [Export(typeof(IFormat))]
    public class TextFormat : Format, IFormat<TextFile>
    {
        #region Methods

        public new TextFile CreateFile(IFolder parent, FileInfo info)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        public override IEnumerable<string> DefaultExtensions
        {
            get { yield return ".txt"; }
        }

        #endregion
    }
}
