using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Nbt.Content;

namespace MCFire.Modules.Nbt.Models
{
    public class NbtFile : ContentHostedFile<NbtContent>
    {
        readonly object _lock = new object();
        public NbtFile([NotNull] IFolder parent, [NotNull] FileInfo info) : base(parent, info) { }

        public override Task OpenAsync()
        {
            return Task.Run(() => IoC.Get<IEventAggregator>().Publish(new FileOpenedMessage<NbtFile>(this)));
        }

        [CanBeNull]
        public NbtContent NbtContent
        {
            get
            {
                lock (_lock)
                {
                    var content = GetContent();
                    if (content != null) return content;

                    // create new NbtContent
                    Stream stream;
                    if (!TryOpenRead(out stream)) return null;
                    // TODO: dependancy injected content types
                    content = new LevelContent(stream);
                    SetContent(content);
                    return content;
                }
            }
        }
    }
}
