using System;
using System.IO;
using MCFire.Modules.Files.Content;

namespace MCFire.Modules.TextEditor.Models
{
    public class TextContent : FileContent
    {
        string _text;
        readonly object _lock = new object();
        public TextContent(Stream stream)
        {
            using (stream)
            {
                try
                {
                    using (var reader = new StreamReader(stream))
                    {
                        _text = reader.ReadToEnd();
                        return;
                    }
                }
                catch (ArgumentException) { }
                catch (IOException) { }
                // ReSharper disable once DoNotCallOverridableMethodsInConstructor
                IsInvalidData();
            }
        }

        public override void Save(Stream stream)
        {
            lock (_lock)
            {
                using (stream)
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(Text);
                    }
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                lock (_lock)
                {
                    _text = value;
                    IsDirty();
                }
            }
        }
    }
}
