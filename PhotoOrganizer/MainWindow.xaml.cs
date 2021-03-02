using MahApps.Metro.Controls;
using PhotoOrganizer.UI.ViewModel;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace PhotoOrganizer.UI
{
    public partial class MainWindow : MetroWindow
    {
        private MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {            
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FadeOutSplashScreen();
            await _viewModel.LoadWorkbenchAsync();
            _viewModel.OpenWorkbenchCommand.Execute(null);
            splashScreen.Visibility = Visibility.Hidden;
        }

        private void FadeOutSplashScreen()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true
            };
            splashScreen.BeginAnimation(OpacityProperty, animation);
        }
    }
}
