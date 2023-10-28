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
        public int BufferLenght { get; } = 30;
        public int Redirects { get; set; } = -1;
        public List<string> BackBuffer { get; set; } = new List<string>();
        public List<string> ForwardBuffer { get; set; } = new List<string>();
        public string CurrentLink { get; set; } = "https://www.google.com/";

        WebBrowser _webBrowser;
        WebView2 _webView;
        Button _refreshButton;
        Button _forwardButton;
        Button _backButton;
        TextBox _searchBar;
        TabItem _tabItem;

        bool _isNewLink = false;

        public WebPage(WebBrowser webBrowser, Button refreshButton, Button forwardButton, Button backButton, TextBox searchBar, TabItem tabItem)
        {
            _webBrowser = webBrowser;
            _refreshButton = refreshButton;
            _forwardButton = forwardButton;
            _backButton = backButton;
            _searchBar = searchBar;
            _tabItem = tabItem;
            InitializeCommon();
            Navigate(CurrentLink);
        }
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
            _backButton.HorizontalAlignment = HorizontalAlignment.Left;
            _backButton.Margin = new Thickness(3, 3, 0, 0);
            _backButton.VerticalAlignment = VerticalAlignment.Top;
            _backButton.Height = 20;
            _backButton.Width = 20;
            _backButton.IsEnabled = false;
            _backButton.Content = "<-";
            _backButton.Click += OnBackButtonClick;

            _forwardButton.HorizontalAlignment = HorizontalAlignment.Left;
            _forwardButton.Margin = new Thickness(28, 3, 0, 0);
            _forwardButton.VerticalAlignment = VerticalAlignment.Top;
            _forwardButton.Height = 20;
            _forwardButton.Width = 20;
            _forwardButton.IsEnabled = false;
            _forwardButton.Content = "->";
            _forwardButton.Click += OnForwardButtonClick;

            _refreshButton.HorizontalAlignment = HorizontalAlignment.Left;
            _refreshButton.Margin = new Thickness(53, 3, 0, 0);
            _refreshButton.VerticalAlignment = VerticalAlignment.Top;
            _refreshButton.Height = 20;
            _refreshButton.Width = 20;
            _refreshButton.Content = "@";
            _refreshButton.Click += OnRefreshButtonClick;

            _searchBar.Margin = new Thickness(85, 3, 29, 0);
            _searchBar.VerticalAlignment = VerticalAlignment.Top;
            _searchBar.Height = 20;
            _searchBar.TextWrapping = TextWrapping.NoWrap;
            _searchBar.KeyDown += OnNavigateBarEnter;
        }

        public async void Initialize()
        {
            _webView.Margin = new Thickness(0, 26, 0, 0);

            await _webView.EnsureCoreWebView2Async();

            _webView.NavigationStarting += OnNavigationStarting;
            _webView.SourceChanged += OnSourceChanged;
            _webView.NavigationCompleted += OnNavigationComplete;

            InitializeCommon();
            Navigate(CurrentLink);
        }


        #region WebBrowser Events
        public void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            _OnNavigating(e.Uri.ToString());
        }
        public void OnNavigated(object sender, NavigationEventArgs e)
        {
            _OnNavigated();
        }
        public void OnLoaded(object sender, EventArgs e)
        {
            ChangeTitle(((HTMLDocument)_webBrowser.Document).title);
        }
        #endregion

        #region WebView2 Events
        public void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            _OnNavigating(e.Uri);
        }
        public void OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            _OnNavigated();
        }
        public void OnNavigationComplete(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            ChangeTitle(_webView.CoreWebView2.DocumentTitle);
        } 
        #endregion


        public void ChangeTitle(string newTitle)
        {
            _tabItem.Header = newTitle;
        }

        public void _OnNavigating(string newUri)
        {
            if (_isNewLink)
            {
                _forwardButton.IsEnabled = false;
                ForwardBuffer.Clear();
                _backButton.IsEnabled = true;
                BackBuffer.Add(CurrentLink);
                if (BackBuffer.Count > BufferLenght)
                {
                    BackBuffer.RemoveAt(0);
                }
                Redirects += 1;
            }
            CurrentLink = newUri;
            _searchBar.Text = CurrentLink;
        }
        public void _OnNavigated()
        {
            if (_isNewLink && Redirects > 0)
            {
                if (Redirects > BufferLenght)
                {
                    Redirects = BufferLenght;
                }
                BackBuffer.RemoveRange(BackBuffer.Count - Redirects, Redirects);
            }
            Redirects = -1;
            _isNewLink = true;
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
                Redirects = -1;
                Navigate(link);
            }
        }

        public void Navigate(string link)
        {
            if (_webBrowser == null)
            {
                _webView.CoreWebView2.Navigate(link);
            }
            else
            {
                _webBrowser.Navigate(link);
            }

        }

        public void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            _forwardButton.IsEnabled = true;
            ForwardBuffer.Add(CurrentLink);
            var link = BackBuffer[BackBuffer.Count-1];
            BackBuffer.Remove(link);
            if (BackBuffer.Count < 1)
            {
                _backButton.IsEnabled = false;
            }
            _isNewLink = false;
            Navigate(link);
        }

        public void OnForwardButtonClick(object sender, RoutedEventArgs e)
        {
            _backButton.IsEnabled = true;
            BackBuffer.Add(CurrentLink);
            var link = ForwardBuffer[ForwardBuffer.Count - 1];
            ForwardBuffer.Remove(link);
            if(ForwardBuffer.Count < 1)
            {
                _forwardButton.IsEnabled = false;
            }
            _isNewLink = false;
            Navigate(link);
        }

        public void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            _isNewLink = false;
            Navigate(CurrentLink);
        }
    }
}
