using Autofac;
using PhotoOrganizer.Common;
using PhotoOrganizer.UI.Resources.Language;
using PhotoOrganizer.UI.Services;
using PhotoOrganizer.UI.Startup;
using PhotoOrganizer.UI.StateMachine;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PhotoOrganizer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ApplicationContext _applicationContext;

        protected override void OnStartup(StartupEventArgs e)
        {
            var dbPath = Path.GetFullPath(FilePaths.ProgramData);
            AppDomain.CurrentDomain.SetData(FilePaths.DataDirectory, dbPath);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Bootstrap();
            base.OnStartup(e);            

            SetAppLanguage();

            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _applicationContext = Bootstrapper.Container.Resolve<ApplicationContext>();
            var mainWindow = Bootstrapper.Container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format(TextResources.UnhandledExceptionOccurred_errorMessage, e.Exception.Message);
            _applicationContext.AddErrorMessage(ErrorTypes.BackupError, errorMessage);
            e.Handled = true; 
        }

        private void SetAppLanguage()
        {
            var settingsHandler = Bootstrapper.Container.Resolve<ISettingsHandler>();

            var actualLanguage = settingsHandler.GetLanguageSettings();

            TextResources.Culture = new CultureInfo(actualLanguage);
        }
    }
}
