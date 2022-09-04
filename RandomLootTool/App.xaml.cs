using RandomVisualizerWPF.Utility;
using System.Windows;

namespace RandomVisualizerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            DataManager.Instance.Initialize();
            ImageManager.Instance.Initialize();
        }
    }
}
