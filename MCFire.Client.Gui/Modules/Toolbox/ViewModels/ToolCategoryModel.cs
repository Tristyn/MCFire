using Caliburn.Micro;

namespace MCFire.Client.Gui.Modules.Toolbox.ViewModels
{
    public class ToolCategoryModel
    {
        public ToolCategoryModel()
        {
            Children = new BindableCollection<ToolModel>();
        }

        public string Name { get; set; }
        public BindableCollection<ToolModel> Children { get; set; }
    }
}
