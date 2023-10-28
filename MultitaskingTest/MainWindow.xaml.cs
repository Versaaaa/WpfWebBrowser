using Microsoft.Web.WebView2.Wpf;
using mshtml;
using MultitaskingTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace MultitaskingTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<WebPage> webPages = new List<WebPage>();
        List<TabItem> webTabs = new List<TabItem>();

        public MainWindow()
        {
            InitializeComponent();
            NewTab();
            GeneratorTab();
            TabController.SelectionChanged += ChangedTab;
            TabController.SizeChanged += TabResize;
        }

        public static void HideScriptErrors(WebBrowser wb, bool hide=true)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide);
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        public void TabResize(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < TabController.Items.Count - 1; i++)
            {
                webTabs[i].Width = (TabController.ActualWidth - 21) / webTabs.Count;
            }
        }

        public void ChangedTab(object sender, SelectionChangedEventArgs e)
        {
            if (TabController.Items.Count == webTabs.Count && webTabs[webTabs.Count - 1].IsSelected)
            {
                TabController.Items.RemoveAt(webTabs.Count - 1);
                webTabs.RemoveAt(webTabs.Count - 1);
                NewTab();
                GeneratorTab();
                ((TabItem) TabController.Items[TabController.Items.Count-2]).IsSelected = true;
                TabResize("", new RoutedEventArgs());
            }
        }

        public void GeneratorTab()
        {
            var generatorTab = new TabItem();
            generatorTab.Header = "+";
            TabController.Items.Add(generatorTab);
            webTabs.Add(generatorTab);
        }

        public void NewTab()
        {
            Color FFE5E5E5 = new Color();

            WebBrowser webBrowser = new WebBrowser();
            WebView2 webView = new WebView2();

            Grid grid = new Grid();
            Button backButton = new Button();
            Button forwardButton = new Button();
            Button refreshButton = new Button();
            TextBox searchBar = new TextBox();

            TabItem newTab = new TabItem();
            newTab.Header = "New Tab";
            newTab.Width = 200;
            newTab.MaxWidth = 125;

            //WebPage newPage = new WebPage(webBrowser, refreshButton,forwardButton, backButton, searchBar, newTab);
            WebPage newPage = new WebPage(webView, refreshButton, forwardButton, backButton, searchBar, newTab);

            FFE5E5E5.R = 229;
            FFE5E5E5.G = 229;
            FFE5E5E5.B = 229;
            FFE5E5E5.A = 255;

            webBrowser.Margin = new Thickness(0, 26, 0, 0);
            webBrowser.Navigating += newPage.OnNavigating;
            webBrowser.Navigated += newPage.OnNavigated;
            webBrowser.LoadCompleted += newPage.OnLoaded;
            HideScriptErrors(webBrowser);

            grid.Background = new SolidColorBrush(FFE5E5E5);
            grid.Children.Add(backButton);
            grid.Children.Add(forwardButton);
            grid.Children.Add(refreshButton);
            grid.Children.Add(searchBar);
            //grid.Children.Add(webBrowser);
            grid.Children.Add(webView);


            newTab.Content = grid;

            webPages.Add(newPage);
            webTabs.Add(newTab);

            TabController.Items.Add(webTabs[webTabs.Count-1]);
            
        }

        private void CoreWebView2_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
