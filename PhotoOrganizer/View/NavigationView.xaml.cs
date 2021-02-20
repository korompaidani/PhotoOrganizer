using PhotoOrganizer.UI.ViewModel;
using System.Windows.Controls;

namespace PhotoOrganizer.UI.View
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl
    {
        public NavigationView()
        {
            InitializeComponent();
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)e.OriginalSource;
            if (scrollViewer.VerticalOffset != 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                ((NavigationViewModel)DataContext).LoadNavigationCommand.Execute(null);
                scrollViewer.ScrollToTop();
            }
        }
    }
}
