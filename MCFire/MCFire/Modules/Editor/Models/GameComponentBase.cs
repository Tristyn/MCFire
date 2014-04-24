using SharpDX.Toolkit;

namespace MCFire.Modules.Editor.Models
{
    public abstract class GameComponentBase : IGameComponent
    {
        public virtual void LoadContent() { }

        public virtual void Update(GameTime time) { }

        public virtual void Draw(GameTime time) { }
        public virtual void Dispose() { }

        public virtual EditorGame Game { set; protected get; }
    }
}
