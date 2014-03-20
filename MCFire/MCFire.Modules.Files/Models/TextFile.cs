using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.Messages;

namespace MCFire.Modules.Files.Models
{
    public class TextFile : File
    {
        #region Fields

        private TextContent _textContent;

        #endregion

        #region Constructor

        public TextFile([NotNull] IFolder parent, [NotNull] FileInfo info)
            : base(parent, info)
        {
        }

        #endregion

        #region Methods

        public override Task OpenAsync()
        {
            return Task.Run(() => IoC.Get<IEventAggregator>().Publish(new FileOpenedMessage<TextFile>(this)));
        }

        [CanBeNull]
        public TextContent GetTextContent()
        {
            lock (Lock)
            {
                if (_textContent != null) return _textContent;

                Stream stream;
                if (TryOpenRead(out stream))
                {
                    return _textContent = new TextContent(stream);
                }
            }
            return null;
        }

        #endregion

        #region Properties

        public TextContent TextContent
        {
            get { return GetTextContent(); }
        }

        #endregion
    }
}
