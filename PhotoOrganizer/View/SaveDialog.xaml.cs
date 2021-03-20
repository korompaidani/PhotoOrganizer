using MahApps.Metro.Controls;
using PhotoOrganizer.UI.ViewModel;
using System.Windows;

namespace PhotoOrganizer.UI.View
{
    /// <summary>
    /// Interaction logic for SaveDialog.xaml
    /// </summary>
    public partial class SaveDialog : MetroWindow
    {
        public SaveDialog()
        {
            InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ((SaveModel)DataContext).SaveCommand.Execute(null);
            this.Close();
        }

        private void SaveAllClick(object sender, RoutedEventArgs e)
        {
            ((SaveModel)DataContext).SaveAllCommand.Execute(null);
            this.Close();
        }

        private void DiscardClick(object sender, RoutedEventArgs e)
        {
            ((SaveModel)DataContext).DiscardCommand.Execute(null);
            this.Close();
        }

        private void DiscardAllClick(object sender, RoutedEventArgs e)
        {
            ((SaveModel)DataContext).DiscardAllCommand.Execute(null);
            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            ((SaveModel)DataContext).CancelCommand.Execute(null);
            this.Close();
        }
    }
}
