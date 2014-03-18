﻿using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure;
using Xceed.Wpf.AvalonDock.Controls;

namespace MCFire.Modules.Files.Models
{
    class UnknownFile : File
    {
        readonly Func<IFile, FileInfo, IFile> _getReplacementFileFunc;

        public UnknownFile([NotNull] IFolder parent, [NotNull] FileInfo info,
            [NotNull] Func<IFile, FileInfo, IFile> getReplacementFileFunc)
            : base(parent, info)
        {
            _getReplacementFileFunc = getReplacementFileFunc;
        }

        public override async Task Open()
        {
            var replacementFile = _getReplacementFileFunc(this, _info);
            if (!await Parent.ReplaceFileWith(this, replacementFile))
                return;

            await replacementFile.Open();
        }
    }
}
