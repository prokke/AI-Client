﻿using CefSharp;
using CefSharp.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfApp2;


namespace AI_Client
{

    public partial class MainWindow : Window
    {
        private ChromiumWebBrowser chromeBrowser;


        readonly Urls urls = new Urls();

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

        private void LoadUrl(string url)
        {

            if (chromeBrowser != null)
            {
                ProggresBar.Visibility = Visibility.Visible;
                chromeBrowser.Address = url;
            }

        }
        private void AI_Selected(object sender, RoutedEventArgs e)
        {

            ListBoxItem selectedItem = (ListBoxItem)sender;
            string caseValue = selectedItem.Name.ToString();

            switch (caseValue)
            {
                case "gpt":
                    LoadUrl(urls._gptUrl);
                    break;
                case "claude":
                    LoadUrl(urls._claudeUrl);
                    break;
                case "gemini":
                    LoadUrl(urls._geminiUrl);
                    break;
                case "ip":
                    LoadUrl(urls._ipUrl);
                    break;
                default:
                    break;
            }
        }
    }
}
