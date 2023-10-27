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

        public void ChangedTab(object sender, SelectionChangedEventArgs e)
        {
            if (TabController.Items.Count == webTabs.Count && ((TabItem)TabController.Items[webTabs.Count - 1]).IsSelected)
            {
                TabController.Items.RemoveAt(webTabs.Count - 1);
                webTabs.RemoveAt(webTabs.Count - 1);
                NewTab();
                GeneratorTab();
                ((TabItem) TabController.Items[TabController.Items.Count-2]).IsSelected = true;
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
            Grid grid = new Grid();
            Button backButton = new Button();
            Button forwardButton = new Button();
            Button refreshButton = new Button();
            TextBox searchBar = new TextBox();

            WebPage newPage = new WebPage(webBrowser, forwardButton, backButton, searchBar);

            TabItem newTab = new TabItem();
            newTab.Header = "TabItem";

            FFE5E5E5.R = 229;
            FFE5E5E5.G = 229;
            FFE5E5E5.B = 229;
            FFE5E5E5.A = 255;

            webBrowser.Margin = new Thickness(0, 26, 0, 0);
            webBrowser.Navigated += newPage.LinkChanged;
            HideScriptErrors(webBrowser);

            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.Margin = new Thickness(3, 3, 0, 0);
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.Height = 20;
            backButton.Width = 20;
            backButton.IsEnabled = false;
            backButton.Content = "<-";
            backButton.Name = $"backButton{webPages.Count}";
            backButton.Click += newPage.Back;

            forwardButton.HorizontalAlignment = HorizontalAlignment.Left;
            forwardButton.Margin = new Thickness(28, 3, 0, 0);
            forwardButton.VerticalAlignment = VerticalAlignment.Top;
            forwardButton.Height = 20;
            forwardButton.Width = 20;
            forwardButton.IsEnabled = false;
            forwardButton.Content = "->";
            forwardButton.Name = $"forwardButton{webPages.Count}";
            forwardButton.Click += newPage.Forward;

            refreshButton.HorizontalAlignment = HorizontalAlignment.Left;
            refreshButton.Margin = new Thickness(53, 3, 0, 0);
            refreshButton.VerticalAlignment = VerticalAlignment.Top;
            refreshButton.Height = 20;
            refreshButton.Width = 20;
            refreshButton.Content = "@";
            refreshButton.Name = $"refreshButton{webPages.Count}";
            refreshButton.Click += newPage.Refresh;

            searchBar.Margin = new Thickness(85, 3, 29, 0);
            searchBar.VerticalAlignment = VerticalAlignment.Top;
            searchBar.Height = 20;
            searchBar.TextWrapping = TextWrapping.NoWrap;
            searchBar.Name = $"searchBar{webPages.Count}"; 
            searchBar.KeyDown += newPage.NavigateBar;

            grid.Background = new SolidColorBrush(FFE5E5E5);
            grid.Children.Add(backButton);
            grid.Children.Add(forwardButton);
            grid.Children.Add(refreshButton);
            grid.Children.Add(searchBar);
            grid.Children.Add(webBrowser);

            newTab.Content = grid;

            webPages.Add(newPage);
            webTabs.Add(newTab);

            TabController.Items.Add(webTabs[webTabs.Count-1]);

        }
    }
}
