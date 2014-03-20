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
            _textContent = file.TextContent;
        }

        public string Text
        {
            get { return _textContent.Text; }
            set { _textContent.Text = value; }
        }
    }
}
