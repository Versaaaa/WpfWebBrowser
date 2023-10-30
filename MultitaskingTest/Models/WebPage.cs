using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using mshtml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MultitaskingTest.Models
{
    public class WebPage
    {
        public string CurrentLink { get; set; } = "https://www.google.com/";

        WebView2 _webView;
        Button _refreshButton;
        Button _forwardButton;
        Button _backButton;
        TextBox _searchBar;
        TabItem _tabItem;

        public WebPage(WebView2 webView, Button refreshButton, Button forwardButton, Button backButton, TextBox searchBar, TabItem tabItem)
        {
            _webView = webView;
            _refreshButton = refreshButton;
            _forwardButton = forwardButton;
            _backButton = backButton;
            _searchBar = searchBar;
            _tabItem = tabItem;
            Initialize();
        }

        public void InitializeCommon()
        {
            _tabItem.Header = "New Tab";
            _tabItem.MaxWidth = 125;
            _tabItem.Width = 200;

            _backButton.HorizontalAlignment = HorizontalAlignment.Left;
            _backButton.Margin = new Thickness(3, 3, 0, 0);
            _backButton.VerticalAlignment = VerticalAlignment.Top;
            _backButton.Height = 20;
            _backButton.Width = 20;
            _backButton.IsEnabled = false;
            _backButton.Content = "<-";

            _forwardButton.HorizontalAlignment = HorizontalAlignment.Left;
            _forwardButton.Margin = new Thickness(28, 3, 0, 0);
            _forwardButton.VerticalAlignment = VerticalAlignment.Top;
            _forwardButton.Height = 20;
            _forwardButton.Width = 20;
            _forwardButton.IsEnabled = false;
            _forwardButton.Content = "->";

            _refreshButton.HorizontalAlignment = HorizontalAlignment.Left;
            _refreshButton.Margin = new Thickness(53, 3, 0, 0);
            _refreshButton.VerticalAlignment = VerticalAlignment.Top;
            _refreshButton.Height = 20;
            _refreshButton.Width = 20;
            _refreshButton.Content = "@";

            _searchBar.Margin = new Thickness(85, 3, 29, 0);
            _searchBar.VerticalAlignment = VerticalAlignment.Top;
            _searchBar.Height = 20;
            _searchBar.TextWrapping = TextWrapping.NoWrap;

            _webView.Margin = new Thickness(0, 26, 0, 0);
        }

        public async void Initialize()
        {
            InitializeCommon();

            await _webView.EnsureCoreWebView2Async();

            _backButton.Click += OnBackButtonClick;
            _forwardButton.Click += OnForwardButtonClick;
            _refreshButton.Click += OnRefreshButtonClick;
            _searchBar.KeyDown += OnNavigateBarEnter;

            _webView.CoreWebView2.NavigationStarting += OnNavigationStarting;
            _webView.CoreWebView2.NavigationCompleted += OnNavigationComplete;
            _webView.CoreWebView2.HistoryChanged += OnHistoryChanged;

            Navigate(CurrentLink);
        }

        #region WebView2 Events
        public void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            _OnNavigating(e.Uri);
        }
        public void OnNavigationComplete(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            ChangeTitle(_webView.CoreWebView2.DocumentTitle);
        } 

        public void OnHistoryChanged(object sender, object e)
        {
            _forwardButton.IsEnabled = _webView.CoreWebView2.CanGoForward;
            _backButton.IsEnabled = _webView.CoreWebView2.CanGoBack;
        }

        #endregion
        public void ChangeTitle(string newTitle)
        {
            _tabItem.Header = newTitle;
        }

        public void _OnNavigating(string newUri)
        {
            CurrentLink = newUri;
            _searchBar.Text = CurrentLink;
        }

        public void OnNavigateBarEnter(object sender, RoutedEventArgs e)
        {
            if (((KeyEventArgs)e).Key == Key.Enter) 
            { 
                var link = ((TextBox)sender).Text;
                
                Regex regex = new Regex(@"^(https?:\/\/)(www\.)?[a-zA-Z0-9]{2,}(\.[a-zA-Z0-9]{2,})(\.[a-zA-Z0-9]{2,})?((\/)(([^ ])+)?)?$", RegexOptions.IgnoreCase);
                if (!regex.IsMatch(link))
                {
                    link = "https://www.google.com/search?q=" + link;
                }
                Navigate(link);
            }
        }

        public void Navigate(string link)
        {
            _webView.CoreWebView2.Navigate(link);
        }

        public void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            _webView.CoreWebView2.GoBack();
        }

        public void OnForwardButtonClick(object sender, RoutedEventArgs e)
        {
            _webView.CoreWebView2.GoForward();
        }

        public void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            _webView.CoreWebView2.Reload();
        }
    }
}
