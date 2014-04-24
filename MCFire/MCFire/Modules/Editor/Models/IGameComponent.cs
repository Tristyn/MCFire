using System;
using SharpDX.Toolkit;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// A non-shared MEF component that is used to interface directly with an EditorGame
    /// </summary>
    public interface IGameComponent : IDisposable
    {
        void LoadContent();
        void Update(GameTime time);
        void Draw(GameTime time);
        EditorGame Game { set; }
    }
}
