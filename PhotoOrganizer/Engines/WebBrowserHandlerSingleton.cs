using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace PhotoOrganizer.UI.Engine
{
    public class WebBrowserHandlerSingleton
    {
        private static WebBrowserHandlerSingleton _instance = null;

        public static WebBrowserHandlerSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WebBrowserHandlerSingleton();
                }
                return _instance;
            }
        }

        public static WebBrowser ControlInstance
        {
            get { return _instance.Control; }
        }

        public WebBrowser Control { get; private set; }

        public bool IsReady { get; private set; }

        private DispatcherFrame _initializeFrame;

        private Window _initializeWindow;

        private WebBrowserHandlerSingleton()
        {
        }

        public void Initialize()
        {
            Control = new WebBrowser();

            ScrollViewer.SetHorizontalScrollBarVisibility(Control, ScrollBarVisibility.Disabled);
            ScrollViewer.SetVerticalScrollBarVisibility(Control, ScrollBarVisibility.Auto);

            Control.Navigated += ControlOnInitialNavigated;
            Control.LoadCompleted += ControlOnInitialLoadCompleted;
            Control.MouseWheel += ControlOnMouseWheel;
            Control.Navigate(new Uri("https://www.google.com/maps"));

            _initializeWindow = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ShowActivated = false,
                Content = Control
            };
            _initializeWindow.Show();

            Dispatcher.PushFrame(_initializeFrame = new DispatcherFrame());
        }

        private void ControlOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MessageBox.Show("Mouse wheel");
        }

        private static void ControlOnNavigating(object sender, NavigatingCancelEventArgs args)
        {
            args.Cancel = true;

            Process.Start(args.Uri.ToString());
        }

        private void ControlOnInitialNavigated(object sender, NavigationEventArgs args)
        {
            SetSilent(Control, true);

            Control.Navigated -= ControlOnInitialNavigated;
            Control.Navigating += ControlOnNavigating;
        }

        private void ControlOnInitialLoadCompleted(object sender, NavigationEventArgs args)
        {
            Control.LoadCompleted -= ControlOnInitialLoadCompleted;

            IsReady = true;
            _initializeFrame.Continue = false;
            _initializeWindow.Close();

            DisableJavaScriptErrors
        }

        private void DisableJavaScriptErrors()
        {
            dynamic document = Control.Document;

            if (document.readyState != "complete")
                return;

            dynamic script = document.createElement("script");
            script.type = @"text/javascript";
            script.text = @"window.onerror = function(msg,url,line){return true;}";
            document.head.appendChild(script);
        }

        private static void SetSilent(WebBrowser control, bool silent)
        {
            var serviceProvider = control.Document as IOleServiceProvider;

            if (serviceProvider == null)
            {
                return;
            }

            var IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
            var IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

            object webBrowser2;
            serviceProvider.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser2);
            if (webBrowser2 == null)
            {
                return;
            }

            webBrowser2.GetType().InvokeMember(
              "Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null,
              webBrowser2, new object[] { silent });            
        }

        private interface IOleServiceProvider
        {
            void QueryService(ref Guid iID_IWebBrowserApp, ref Guid iID_IWebBrowser2, out object webBrowser2);
        }
    }
}
