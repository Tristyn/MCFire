using System;
using System.IO;
using MCFire.Modules.Files.Events;

namespace MCFire.Modules.Files.Content
{
    public abstract class FileContent : IFileContent
    {
        #region Properties

        private bool _dirty;

        #endregion

        #region Constructor

        protected FileContent()
        {
            _dirty = false;
        }

        #endregion

        #region Methods

        public void Save()
        {
            OnSaved(new FileContentEventArgs(this));
        }

        public abstract void Save(Stream stream);

        protected void IsDirty()
        {
            if(_dirty) return;
            _dirty = true;
            OnDirtied(new FileContentEventArgs(this));
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

        #endregion

        #region Properties

        public bool Dirty
        {
            get { return _dirty; }
        }
        // TODO: event handler with proper args
        public event EventHandler<FileContentEventArgs> Dirtied;
        public event EventHandler<FileContentEventArgs> Saved;

        #endregion
    }
}
