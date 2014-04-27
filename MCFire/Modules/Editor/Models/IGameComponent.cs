using MCFire.Modules.Editor.Models.MCFire.Modules.Infrastructure.Interfaces;
using SharpDX.Toolkit;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// A non-shared MEF component that is used to interface directly with an EditorGame.
    /// It is recommended that a game component should be backed by MEF services.
    /// </summary>
    public interface IGameComponent : ICleanup
    {
        void LoadContent();
        void Update(GameTime time);
        void Draw(GameTime time);
        /// <summary>
        /// The order to draw each component. Components that want to be rendered later in the pipeline have a higher DrawPriority.
        /// The default is 100. Opaques get drawn at 20, the transparent box selector draws at 500.
        /// </summary>
        int DrawPriority { get; }
        EditorGame Game { set; }
    }

    namespace MCFire.Modules.Infrastructure.Interfaces
    {
        /// <summary>
        /// A copy of IDisposable.
        /// ICleanup gets around the issue of IDisposable references being held by MEF.
        /// </summary>
        public interface ICleanup
        {
            void Dispose();
        }
    }
}
