using System;
using System.ComponentModel.Composition;
using Gemini.Framework;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.TextEditor.ViewModels
{
    [Export]
    public class TextEditorViewModel : Document
    {
        private readonly TextContent _textContent;

        public TextEditorViewModel(TextFile file)
        {
            DisplayName = file.Name;
            _textContent = file.TextContent;
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
