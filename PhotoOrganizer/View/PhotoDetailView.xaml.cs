using PhotoOrganizer.UI.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhotoOrganizer.UI.View
{
    /// <summary>
    /// Interaction logic for PhotoDetailView.xaml
    /// </summary>
    public partial class PhotoDetailView : UserControl
    {
        public PhotoDetailView()
        {
            InitializeComponent();
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((PhotoDetailViewModel)DataContext).OpenPhotoCommand.Execute(null);
        }

        private void OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
        }
    }
}
