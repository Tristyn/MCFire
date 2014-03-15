using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Models
{
    class UnknownFile : File
    {
        readonly Func<IFile, IFormat> _formatDeterminerFunc;

        public UnknownFile([NotNull] IFolder parent, [NotNull] FileInfo info,
            [NotNull] Func<IFile, IFormat> formatDeterminerFunc)
            : base(parent, info)
        {
            _formatDeterminerFunc = formatDeterminerFunc;
        }

        public override async Task Open()
        {
            var format = _formatDeterminerFunc(this);

            if (format != null)
                await ReplaceWith(format);
        }
    }
}
