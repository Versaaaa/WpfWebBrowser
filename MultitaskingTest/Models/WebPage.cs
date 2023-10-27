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
        Button _forwardButton;
        Button _backButton;
        TextBox _searchBar;
        TabItem _tabItem;

        bool _isNewLink = true;

        public WebPage(WebBrowser webBrowser, Button forwardButton, Button backButton, TextBox searchBar, TabItem tabItem)
        {
            _webBrowser = webBrowser;
            _forwardButton = forwardButton;
            _backButton = backButton;
            _searchBar = searchBar;
            _tabItem = tabItem;
            Navigate(CurrentLink);
        }

        public void OnLoaded(object sender, EventArgs e)
        {
            _tabItem.Header = ((HTMLDocument)_webBrowser.Document).title;
        }

        public void OnNavigating(object sender, NavigatingCancelEventArgs e)
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
            CurrentLink = e.Uri.ToString();
            _searchBar.Text = CurrentLink;
        }

        public void OnNavigated(object sender, NavigationEventArgs e)
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
            _webBrowser.Navigate(link);
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
