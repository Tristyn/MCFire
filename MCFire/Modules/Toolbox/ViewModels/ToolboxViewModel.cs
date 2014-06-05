using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Toolbox.Services;

namespace MCFire.Modules.Toolbox.ViewModels
{
    [Export]
    public class ToolboxViewModel : Tool
    {
        BindableCollection<ToolCategoryModel> _categories = 
            new BindableCollection<ToolCategoryModel>();

        [Import] ToolboxService _toolbox;

        public ToolboxViewModel()
        {
            DisplayName = "Tools";

            // assign tools to their categories
            var tools = IoC.Get<ToolboxService>().Tools;
            var categories = new BindableCollection<ToolCategoryModel>();

            foreach (var tool in tools)
            {
                var matchingCat = categories.FirstOrDefault(cat => cat.Name == tool.ToolCategory);

                if (matchingCat == null)
                {
                    matchingCat = new ToolCategoryModel();
                    matchingCat.Name = tool.ToolCategory;
                    categories.Add(matchingCat);
                }

                var toolViewModel = new ToolModel(tool);
                matchingCat.Children.Add(toolViewModel);
            }

            Categories = categories;
        }

        #region View

        public void ToolSelected(object item)
        {
            var tool = item as ToolModel;
            if (tool == null) return;
            _toolbox.SetCurrentTool(tool.Tool);
        }

        public void DeselectItem(ListBox dataContext)
        {
            dataContext.SelectedIndex = -1;
        }

        public BindableCollection<ToolCategoryModel> Categories
        {
            get { return _categories; }
            private set
            {
                if (Equals(value, _categories)) return;
                _categories = value;
                NotifyOfPropertyChange(() => Categories);
            }
        }

        #endregion

        #region Gemini

        public override double PreferredWidth
        {
            get { return 150; }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }

        #endregion
    }
}
