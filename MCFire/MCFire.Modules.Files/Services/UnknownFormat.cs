using System;
using System.IO;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Services
{
    class UnknownFormat:Format, IFormat<UnknownFile>
    {
        #region Fields

        readonly Func<IFile, IFormat> _formatDermineFunc;

        #endregion

        #region Constructor

        public UnknownFormat([NotNull] Func<IFile, IFormat> formatDermineFunc)
        {
            if (formatDermineFunc == null) throw new ArgumentNullException("formatDermineFunc");

            _formatDermineFunc = formatDermineFunc;
        }

        #endregion

        #region Methods

        public new UnknownFile CreateFile(IFolder parent, FileInfo info)
        {
            return base.CreateFile(parent, info) as UnknownFile;
        }

        protected override IFile ConstructFile(IFolder parent, FileInfo info)
        {
            return new UnknownFile(parent, info, _formatDermineFunc);
        }

        #endregion
    }
}
