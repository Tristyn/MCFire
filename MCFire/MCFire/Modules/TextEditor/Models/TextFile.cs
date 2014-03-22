using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.Messages;

namespace MCFire.Modules.Files.Models
{
    public class TextFile : ContentHostedFile<TextContent>
    {
        #region Fields

        readonly object _lock = new object();

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
        protected TextContent GetTextContent()
        {
            TextContent content;
            lock (_lock)
            {
                content = Content;
                if (content != null) return content;


                Stream stream;
                if (!TryOpenRead(out stream)) return null;

                content = new TextContent(stream);
            }
            Content = content;
            return content;
        }

        #endregion

        #region Properties

        [CanBeNull]
        public TextContent TextContent
        {
            get { return GetTextContent(); }
        }

        #endregion
    }
}
