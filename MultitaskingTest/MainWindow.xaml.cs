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
using System.Windows.Media;

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
            GeneratorTab();
            NewWebTab(webTabs[webTabs.Count - 1]);
            GeneratorTab();
            TabController.SelectionChanged += ChangedTab;
            TabController.SizeChanged += TabResize;
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
            if (webTabs[webTabs.Count - 1].IsSelected)
            {
                NewWebTab(webTabs[webTabs.Count - 1]);
                GeneratorTab();
                webTabs[webTabs.Count-2].IsSelected = true;
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

        public void NewWebTab(TabItem newTab)
        {
            Grid grid = new Grid();
            Color FFE5E5E5 = new Color();

            WebView2 webView = new WebView2();

            Button backButton = new Button();
            Button forwardButton = new Button();
            Button refreshButton = new Button();
            TextBox searchBar = new TextBox();

            WebPage newPage = new WebPage(webView, refreshButton, forwardButton, backButton, searchBar, newTab);

            FFE5E5E5.R = 229;
            FFE5E5E5.G = 229;
            FFE5E5E5.B = 229;
            FFE5E5E5.A = 255;

            grid.Background = new SolidColorBrush(FFE5E5E5);
            grid.Children.Add(backButton);
            grid.Children.Add(forwardButton);
            grid.Children.Add(refreshButton);
            grid.Children.Add(searchBar);
            grid.Children.Add(webView);

            newTab.Content = grid;

            webPages.Add(newPage);
        }
    }
}