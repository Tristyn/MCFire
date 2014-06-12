using GongSolutions.Wpf.DragDrop;
using MCFire.Common.Infrastructure.DragDrop;
using SharpDX.Toolkit;

namespace MCFire.Graphics.Editor
{
    /// <summary>
    /// A non-shared MEF component that is used to interface directly with an EditorGame.
    /// It is recommended that a game component should be backed by MEF services.
    /// </summary>
    public interface IGameComponent
    {
        void LoadContent(IEditorGame game);
        void UnloadContent();
        void Update(GameTime time);
        void Draw(GameTime time);

        /// <summary>
        /// The order to draw each component. Components that want to be rendered later in the pipeline have a higher DrawPriority.
        /// The default is 100. Opaques get drawn at 20, the transparent box selector draws at 500.
        /// </summary>
        int DrawPriority { get; }

        #region Drag Drop
        #region DragSource
        // TODO: new drag system that doesn't rely on Gong drag drop
        void StartDrag(IHandleableDragInfo dragInfo);
        void Dropped(IDropInfo dropInfo);
        void DragCancelled();

        #endregion

        #region DropTarget

        void DragOver(IDropInfo dropInfo);
        void Drop(IDropInfo dropInfo);

        #endregion

        #endregion

        void WpfKeyDown(System.Windows.Input.KeyEventArgs e);
        void Dispose();
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
