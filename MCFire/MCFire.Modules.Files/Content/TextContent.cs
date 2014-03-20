using System.IO;

namespace MCFire.Modules.Files.Content
{
    public class TextContent : FileContent
    {
        private string _text;
        readonly object _lock = new object();
        public TextContent(Stream stream)
        {
            using (stream)
            {
                using (var reader = new StreamReader(stream))
                {
                    _text = reader.ReadToEnd();
                }
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
