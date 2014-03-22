﻿using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.TextEditor.Models
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

        #endregion

        #region Properties

        [CanBeNull]
        public TextContent TextContent
        {
            get
            {
                lock (_lock)
                {
                    var content = GetContent();
                    if (content != null) return content;

                    // create new TextContent
                    Stream stream;
                    if (!TryOpenRead(out stream)) return null;
                    content = new TextContent(stream);
                    SetContent(content);
                    return content;
                }
            }
        }

        #endregion
    }
}
