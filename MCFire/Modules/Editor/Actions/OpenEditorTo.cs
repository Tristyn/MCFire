using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using MCFire.Modules.Editor.ViewModels;
using MCFire.Modules.Explorer.Models;

namespace MCFire.Modules.Editor.Actions
{
    public class OpenEditorTo : IResult
    {
        [Import]
        EditorViewModel _editor;
        [Import]
        IShell _shell;

        readonly MCFireWorld _world;
        readonly int _dimension;

        public OpenEditorTo(MCFireWorld world, int dimension)
        {
            _world = world;
            _dimension = dimension;
        }

        public void Execute(ActionExecutionContext context)
        {
            _editor.TryInitializeTo(_world, _dimension);
            _shell.OpenDocument(_editor);
            if (Completed != null) Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}