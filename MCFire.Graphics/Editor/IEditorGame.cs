using System;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using MCFire.Common;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor
{
    public interface IEditorGame
    {
        World World { get; }
        int Dimension { get; }
        GraphicsDevice GraphicsDevice { get; }
        Camera Camera { get;  }
        Keyboard Keyboard { get; }
        Mouse Mouse { get; }
        SharpDXElement SharpDxElement { get; }
        T LoadContent<T>(string assetName) where T : class,IDisposable;

        [CanBeNull]
        TComponent GetComponent<TComponent>()
            where TComponent : class;
    }

    public interface IEditorGameFacade : IDisposable, IDragSource, IDropTarget
    {
        void WpfKeyDown(System.Windows.Input.KeyEventArgs e);
    }
}