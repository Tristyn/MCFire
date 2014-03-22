using System;
using System.IO;
using MCFire.Modules.Files.Events;

namespace MCFire.Modules.Files.Content
{
    public abstract class FileContent : IFileContent
    {
        #region Constructor

        protected FileContent()
        {
            ValidData = true;
            Dirty = false;
        }

        #endregion

        #region Methods

        public void Save()
        {
            OnSaved(new FileContentEventArgs(this));
        }

        public abstract void Save(Stream stream);

        /// <summary>
        /// If the program has changed any of the data in this FileContent, and therefore needs to be saved.
        /// </summary>
        protected void IsDirty()
        {
            if(Dirty) return;
            Dirty = true;
            OnDirtied(new FileContentEventArgs(this));
        }

        /// <summary>
        /// Call this if the content has invalid data during loading from stream.
        /// Sets ValidData to false and calls OnInvalidData
        /// Setting this to false calls OnInvalidData
        /// </summary>
        protected void IsInvalidData()
        {
            ValidData = false;
            OnInvalidData(new FileContentEventArgs(this));
        }

        protected virtual void OnDirtied(FileContentEventArgs e)
        {
            var handler = Dirtied;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnSaved(FileContentEventArgs e)
        {
            var handler = Saved;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnInvalidData(FileContentEventArgs e)
        {
            var handler = InvalidData;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Properties

        public bool Dirty { get; private set; }

        /// <summary>
        /// If the content has invalid data during loading.
        /// </summary>
        public bool ValidData { get; private set; }

        public event EventHandler<FileContentEventArgs> Dirtied;
        public event EventHandler<FileContentEventArgs> Saved;
        public event EventHandler<FileContentEventArgs> InvalidData;

        #endregion
    }
}
