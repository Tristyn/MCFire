using System;
using System.ComponentModel.Composition;
using Gemini.Framework;
using MCFire.Modules.Files.Models;
using MCFire.Modules.TextEditor.Models;

namespace MCFire.Modules.TextEditor.ViewModels
{
    [Export]
    public class TextEditorViewModel : Document
    {
        private readonly TextContent _textContent;

        public TextEditorViewModel(IFile file, TextContent content)
        {
            DisplayName = file.Name;
            _textContent = content;
        }

        public override void CanClose(Action<bool> callback)
        {
            callback(!_textContent.Dirty);
        }

        public string Text
        {
            get { return _textContent.Text; }
            set { _textContent.Text = value; }
        }
    }
}
