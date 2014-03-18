using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Framework
{
    public class FileRefreshedEventArgs : EventArgs
    {
        public FileRefreshedEventArgs(IFile file)
        {
            File = file;
        }

        public IFile File { get; set; }
    }
}
