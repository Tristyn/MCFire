using System.IO;
using JetBrains.Annotations;

namespace MCFire.Modules.Files.Models
{
    public class TextFile : File
    {
        protected TextFile([NotNull] Folder parent, [NotNull] FileInfo info)
            : base(parent, info)
        {
        }
    }
}
