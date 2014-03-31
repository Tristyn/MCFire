using System;
using System.Collections.Generic;
using System.IO;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Nbt.Attributes;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.Models
{
    public abstract class NbtContent : FileContent
    {
        bool _initializing = true;

        public override bool Load(Stream stream)
        {
            using (stream)
            {
                var nbtFile = new NbtFile();
                try
                {
                    nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
                    // Inherited properties will get assigned here.
                    NbtBuilder.NbtBuilder.BuildExisting(this, nbtFile.RootTag);
                    _initializing = false;
                    return true;
                }
                catch (ArgumentOutOfRangeException) { }
                catch (NotSupportedException) { }
                catch (EndOfStreamException) { }
                catch (InvalidDataException) { }
                catch (NbtFormatException) { }

                IsInvalidData();
                return false;
            }
        }
        // TODO: have list of nbtTag that gets included when saving. any fields that arent assigned during loaded are put there, then included when saving.
        public override void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        protected override void IsDirty()
        {
            if (_initializing) return;
            base.IsDirty();
        }

        [AuxiliaryTags]
        public List<NbtTag> ExtraTags { get; set; }
    }
}
