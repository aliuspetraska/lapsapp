using System;
using LapsMobileApp.Models;
using Xamarin.Forms;

namespace LapsMobileApp.Pages
{
    public partial class TrackPage : ContentPage
    {
        private Track _track;

        public TrackPage(Track track)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "Back");

            _track = track;

            webView.Navigated += (sender, e) => 
            {
                Console.WriteLine("1. " + e.Url);
                Console.WriteLine("2. " + e.Source);
            };

            webView.Navigating += (sender, e) => 
            {
                Console.WriteLine("3. " + e.Url);
                Console.WriteLine("4. " + e.Source);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            webView.IsVisible = false;
            activityIndicator.IsVisible = true;


        }
    }
}
