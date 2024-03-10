using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AI_Client
{

    public partial class MainWindow : Window
    {
        private ChromiumWebBrowser chromeBrowser;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void InitializeChromium()
        {
            try
            {
                CefSettings settings = new CefSettings
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36 /CefSharp Browser" + Cef.CefSharpVersion,
                    CachePath = Directory.GetCurrentDirectory() + "\\cache"

                };


                Cef.Initialize(settings);

                chromeBrowser = new ChromiumWebBrowser();
                browsergrid.Children.Add(chromeBrowser); 
                Canvas.SetLeft(chromeBrowser, 0);
                Canvas.SetTop(chromeBrowser, 0);
                chromeBrowser.Address = "about:blank"; 
                chromeBrowser.FrameLoadEnd += ChromeBrowser_FrameLoadEnd;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации Chromium: {ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void ChromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Dispatcher.Invoke(() => ProggresBar.Visibility = Visibility.Hidden);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
