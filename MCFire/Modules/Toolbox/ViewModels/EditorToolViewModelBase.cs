using System.ComponentModel.Composition;
using Caliburn.Micro;
using MCFire.Modules.Toolbox.Messages;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public abstract class EditorToolViewModelBase<TTool> : PropertyChangedBase, IEditorToolViewModel
        where TTool : IEditorTool
    {
        [Import] IEventAggregator _aggregator;

        /// <summary>
        /// When the tool has been clicked, notify the application.
        /// </summary>
        public void Clicked()
        {
            _aggregator.Publish(new EditorToolSelectedMessage(typeof(TTool)));
        }
    }
}
