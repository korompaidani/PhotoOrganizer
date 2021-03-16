using MahApps.Metro.Controls;
using PhotoOrganizer.UI.ViewModel;
using System;
using System.Windows;

namespace PhotoOrganizer.UI.View
{
    /// <summary>
    /// Interaction logic for WritingToFileView.xaml
    /// </summary>
    public partial class WritingToFileView : MetroWindow
    {
        WritingToFileViewModel _viewModel;

        public WritingToFileView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as WritingToFileViewModel;
            _viewModel.ProgressBar = WriteStatusBar;
        }

        private void WindowContentRendered(object sender, EventArgs e)
        {
            _viewModel.WindowContentRendered(sender, e);
        }

    }
}
