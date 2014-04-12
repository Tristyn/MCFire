using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace MCFire.Modules.Startup.Views
{
    /// <summary>
    /// Interaction logic for ConfigureInstallationsView.xaml
    /// </summary>
    public partial class ConfigureInstallationsView
    {
        public ConfigureInstallationsView()
        {
            Loaded += LoadAnimations;
            InitializeComponent();
        }

        async void LoadAnimations(object sender, RoutedEventArgs e)
        {
            var anim = FindResource(@"AnimateMargin50DownTo0") as Storyboard;
            if (anim == null) return;

            // set all elements to be invisible
            foreach (var element in LowerPanel.Children.OfType<FrameworkElement>())
            {
                element.Margin = new Thickness(0,50,0,0);
            }

            // play cascading animation
            foreach (var element in LowerPanel.Children.OfType<FrameworkElement>())
            {
                await Task.Delay(250);
                element.BeginStoryboard(anim);
            }
        }
    }
}
