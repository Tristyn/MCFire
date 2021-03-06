using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using MCFire.Client.Gui.Modules.Editor.ViewModels;
using MCFire.Common;

namespace MCFire.Client.Gui.Modules.Editor.Actions
{
    public class OpenEditorTo : IResult
    {
        [Import]
        EditorViewModel _editor;
        [Import]
        IShell _shell;

        readonly World _world;
        readonly int _dimension;

        public OpenEditorTo(World world, int dimension)
        {
            _world = world;
            _dimension = dimension;
        }

        public void Execute(ActionExecutionContext context)
        {
            _shell.OpenDocument(_editor);
            _editor.TryInitializeTo(_world, _dimension);
            if (Completed != null) Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}