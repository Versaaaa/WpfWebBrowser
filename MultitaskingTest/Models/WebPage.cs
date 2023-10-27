using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace MultitaskingTest.Models
{
    public class WebPage
    {
        public int BufferLenght { get; set; } = 30;
        public List<string> BackBuffer { get; set; } = new List<string>();
        public List<string> ForwardBuffer { get; set; } = new List<string>();
        public string CurrentLink { get; set; } = "https://www.google.com/";

        WebBrowser _webBrowser;
        Button _forwardButton;
        Button _backButton;
        TextBox _searchBar;

        bool _AllowLinkChange = false;

        public WebPage(WebBrowser webBrowser, Button forwardButton, Button backButton, TextBox searchBar)
        {
            _webBrowser = webBrowser;
            _forwardButton = forwardButton;
            _backButton = backButton;
            _searchBar = searchBar;
            Navigate();
        }

        public void LinkChanged(object sender, NavigationEventArgs e)
        {
            if (_AllowLinkChange)
            {
                _forwardButton.IsEnabled = false;
                ForwardBuffer.Clear();
                _backButton.IsEnabled = true;
                BackBuffer.Add(CurrentLink);
            }
            CurrentLink = _webBrowser.Source.ToString();
            _searchBar.Text = CurrentLink;
            _AllowLinkChange = true;
        }

        public void NavigateBar(object sender, RoutedEventArgs e)
        {
            if (((KeyEventArgs)e).Key == Key.Enter) 
            { 
                _forwardButton.IsEnabled = false;
                ForwardBuffer.Clear();
                _backButton.IsEnabled = true;
                BackBuffer.Add(CurrentLink);

                CurrentLink = ((TextBox)sender).Text;
                
                Regex regex = new Regex(@"^(https?:\/\/)[a-zA-Z0-9]{2,}(\.[a-zA-Z0-9]{2,})(\.[a-zA-Z0-9]{2,})?((\/)(([^ ])+)?)?$", RegexOptions.IgnoreCase);
                if (!regex.IsMatch(CurrentLink))
                {
                    CurrentLink = "https://www.google.com/search?q=" + CurrentLink;
                }

                _AllowLinkChange = false;
                
                Navigate();
            }
        }

        public void Navigate(string link = "")
        {
            _webBrowser.Navigate(CurrentLink);
        }

        public void Back(object sender, RoutedEventArgs e)
        {
            _forwardButton.IsEnabled = true;
            ForwardBuffer.Add(CurrentLink);
            CurrentLink = BackBuffer[BackBuffer.Count-1];
            BackBuffer.Remove(CurrentLink);
            if (BackBuffer.Count < 1)
            {
                _backButton.IsEnabled = false;
            }
            _AllowLinkChange = false;
            Navigate();
        }

        public void Forward(object sender, RoutedEventArgs e)
        {
            _backButton.IsEnabled = true;
            BackBuffer.Add(CurrentLink);
            CurrentLink = ForwardBuffer[ForwardBuffer.Count - 1];
            ForwardBuffer.Remove(CurrentLink);
            if(ForwardBuffer.Count < 1)
            {
                _forwardButton.IsEnabled = false;
            }
            _AllowLinkChange = false;
            Navigate();
        }

        public void Refresh(object sender, RoutedEventArgs e)
        {
            _AllowLinkChange = false;
            Navigate();
        }

    }
}
