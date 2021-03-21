using Autofac;
using PhotoOrganizer.UI.Resources.Language;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace PhotoOrganizer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Bootstrap();
            base.OnStartup(e);

            var settingsHandler = Bootstrapper.Container.Resolve<ISettingsHandler>();

            TextResources.Culture = new CultureInfo("hu-HU");

            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var applicationContext = Bootstrapper.Container.Resolve<ApplicationContext>();
            var mainWindow = Bootstrapper.Container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            e.Handled = true;
        }
    }
}
