using PhotoOrganizer.UI.Event;
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
        public event GetCoordinateEventHandler CoordinateHasBeenSet;
        public delegate void GetCoordinateEventHandler(object sender, GetBrowserDataEventArgs args);

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

            CoordinateHasBeenSet += ((MapViewModel)DataContext).OnGetBrowserData;
        }

        private void OnSetCoordinateButtonClick(object sender, RoutedEventArgs e)
        {
            var browserDataArgs = new GetBrowserDataEventArgs
            {
                Url = mapBrowser.Source.ToString()
            };

            GetCoordinateEventHandler handler = CoordinateHasBeenSet;
            handler?.Invoke(this, browserDataArgs);
        }
    }
}
