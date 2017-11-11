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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
