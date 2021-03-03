using PhotoOrganizer.UI.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PhotoOrganizer.UI.View
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
        }

        private void OnLoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var browser = sender as WebBrowser;

            if (browser == null || browser.Document == null)
                return;

            dynamic document = browser.Document;

            if (document.readyState != "complete")
                return;

            dynamic script = document.createElement("script");
            script.type = @"text/javascript";
            script.text = @"window.onerror = function(msg,url,line){return true;}";
            document.head.appendChild(script);
        }

        private void OnSetCoordinateButtonClick(object sender, RoutedEventArgs e)
        {
            ((MapViewModel)DataContext).OnSetCoordinatesOnPhotoOnlyCommand.Execute(mapBrowser.Source.ToString());
        }

        private void OnSaveOverrideButtonClick(object sender, RoutedEventArgs e)
        {
            ((MapViewModel)DataContext).OnSaveOverrideLocationCommand.Execute(mapBrowser.Source.ToString());
        }

        private void OnSaveAsNewButtonClick(object sender, RoutedEventArgs e)
        {
            ((MapViewModel)DataContext).OnSaveAsNewLocationCommand.Execute(mapBrowser.Source.ToString());
        }
    }
}
