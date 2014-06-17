using System;
using GongSolutions.Wpf.DragDrop;
using MCFire.Common;
using MCFire.Graphics.Modules.Editor.Models;
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
        Tasks Tasks { get; }
        SharpDXElement SharpDxElement { get; }
        T LoadContent<T>(string assetName) where T : class,IDisposable;
    }

    public interface IEditorGameFacade : IDisposable, IDragSource, IDropTarget
    {
        void WpfKeyDown(System.Windows.Input.KeyEventArgs e);
    }
}