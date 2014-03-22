using System;
using System.IO;
using JetBrains.Annotations;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.Events;

namespace MCFire.Modules.Files.Models
{
    /// <summary>
    /// Provides services for integrating IFileContent into File.
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    public abstract class ContentHostedFile<TContent> : File where TContent : class, IFileContent
    {
        #region Properties

        [CanBeNull]
        TContent _content;
        [CanBeNull]
        WeakReference<TContent> _weakContentReference;
        /// <summary> Lock for the fields _content and _weakContentReference </summary>
        readonly object _refLock = new object();

        #endregion

        #region Constructor

        protected ContentHostedFile([NotNull] IFolder parent, [NotNull] FileInfo info)
            : base(parent, info) { }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the content for this file. 
        /// The file will take responsibility of saving the contents when saving is requested.
        /// </summary>
        /// <param name="content">The new content to set.</param>
        protected void SetContent([NotNull] TContent content)
        {
            if (content == null) throw new ArgumentNullException("content");
            lock (_refLock)
            {
                if (_weakContentReference == null)
                    _weakContentReference = new WeakReference<TContent>(content);
                else
                {
                    TContent oldContent;
                    if (_weakContentReference.TryGetTarget(out oldContent))
                    {
                        // remove old delegates
                        oldContent.Dirtied -= OnContentDirtied;
                        oldContent.Saved -= OnContentSaved;
                    }
                    _weakContentReference.SetTarget(content);
                }
                content.Dirtied += OnContentDirtied;
                content.Saved += OnContentSaved;
            }
        }

        [CanBeNull]
        protected TContent GetContent()
        {
            TContent result;
            lock (_refLock)
            {
                if (_weakContentReference == null) return null;
                _weakContentReference.TryGetTarget(out result);
            }
            return result;
        }

        /// <summary>
        /// Gets a strong reference to the content, so it can be saved later.
        /// </summary>
        void OnContentDirtied(object sender, FileContentEventArgs e)
        {
            lock (_refLock)
            {
                TContent content = null;
                if (_weakContentReference != null && !_weakContentReference.TryGetTarget(out content)) return;
                if (e.Content != content) return; // This is a ghost event from previously set content. ignore it
                _content = content;
            }
        }

        /// <summary>
        /// Saves the content, then removes our strong reference to it.
        /// </summary>
        void OnContentSaved(object sender, FileContentEventArgs e)
        {
            lock (_refLock)
            {
                if (_content != null)
                    if (_content != e.Content) return;

                // save to file.

                _content = null;
            }
        }

        #endregion
    }
}