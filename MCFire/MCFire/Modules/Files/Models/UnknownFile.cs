using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

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

        public override async Task OpenAsync()
        {
            var replacementFile = _getReplacementFileFunc(this, Info);
            if (!await Parent.ReplaceFileWithAsync(this, replacementFile))
                return;

            await replacementFile.OpenAsync();
        }
    }
}
