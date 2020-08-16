using DevExpress.Xpf.Core;
using System.Windows;

namespace BankApplication {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e)
        {
            ApplicationThemeHelper.ApplicationThemeName = Theme.Office2016ColorfulName;
            base.OnStartup(e);
        }
    }
}