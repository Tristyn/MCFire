using System;
using System.IO;
using MCFire.Modules.Files.Events;

namespace MCFire.Modules.Files.Content
{
    public abstract class FileContent : IDisposable
    {
        #region Properties
        // TODO: derived objects override dispose
        protected bool Disposed;
        private bool _dirty;
        private bool _validData;

        #endregion

        #region Constructor

        protected FileContent()
        {
            ValidData = true;
            Dirty = false;
        }

        ~FileContent()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        public void Save()
        {
            if (Disposed)
                throw new ObjectDisposedException("FileContent");

            OnSaved(new FileContentEventArgs(this));
        }

        public abstract bool Load(Stream stream);

        public abstract void Save(Stream stream);

        /// <summary>
        /// If the program has changed any of the data in this FileContent, and therefore needs to be saved.
        /// </summary>
        protected void IsDirty()
        {
            if (Dirty) return;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Dirtied = null;
                Saved = null;
                InvalidData = null;
            }

            Disposed = true;
        }

        #endregion

        #region Properties

        public bool Dirty
        {
            get
            {
                if (Disposed)
                    throw new ObjectDisposedException("FileContent");

                return _dirty;
            }
            private set { _dirty = value; }
        }

        /// <summary>
        /// If the content has invalid data during loading.
        /// </summary>
        public bool ValidData
        {
            get
            {
                if (Disposed)
                    throw new ObjectDisposedException("FileContent");

                return _validData;
            }
            private set { _validData = value; }
        }

        public event EventHandler<FileContentEventArgs> Dirtied;
        public event EventHandler<FileContentEventArgs> Saved;
        public event EventHandler<FileContentEventArgs> InvalidData;

        #endregion
    }
}
