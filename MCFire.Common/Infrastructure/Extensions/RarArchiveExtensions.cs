using System.IO;

namespace MCFire.Core.Modules.Infrastructure.Extensions
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
