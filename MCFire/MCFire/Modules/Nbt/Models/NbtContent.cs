using System;
using System.IO;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.Content
{
    public abstract class NbtContent : FileContent
    {
        protected NbtContent(Stream stream)
        {
            using (stream)
            {
                var nbtFile = new NbtFile();
                try
                {
                    nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
                    // Inherited properties will get assigned here.
                    NbtBuilder.BuildExisting(this, nbtFile.RootTag);
                    return;
                }
                catch (ArgumentOutOfRangeException) { }
                catch (NotSupportedException) { }
                catch (EndOfStreamException) { }
                catch (InvalidDataException) { }
                catch (NbtFormatException) { }

                IsInvalidData();
            }
        }

        public override void Save(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
