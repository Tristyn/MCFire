using System;
using System.IO;
using NUnrar.Archive;
using NUnrar.Common;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public class RarArchiveExtensions
    {
        public void aasd()
        {
            var archive = RarArchive.Open(@"D:\Archives\Test.rar");
            foreach (var entry in archive.Entries)
            {
                    var fileName = Path.GetFileName(entry.FilePath);
                    var rootToFile = Path.GetFullPath(entry.FilePath).Replace(fileName, "");

                    if (!Directory.Exists(rootToFile))
                    {
                        Directory.CreateDirectory(rootToFile);
                    }

                    entry.WriteToFile(rootToFile + fileName, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
            }
        }
    }
}
