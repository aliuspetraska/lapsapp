using System.Collections.Generic;
using LapsMobileApp.Models;
using Newtonsoft.Json;
using RestSharp;
using Xamarin.Forms;

namespace LapsMobileApp.Pages
{
    public partial class TracksPage : ContentPage
    {
        private List<Track> tracks = new List<Track>();

        private RestClient restClient = new RestClient("http://lapsapp.mybluemix.net");

        public TracksPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "Back");

            listView.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) => 
            {
                if (e.SelectedItem != null) 
                {
                    var track = e.SelectedItem as Track;
                    await Navigation.PushAsync(new TrackPage(track));
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            listView.SelectedItem = null;

            listView.IsVisible = false;
            activityIndicator.IsVisible = true;

            if (tracks.Count < 1)
            {
                var request = new RestRequest("api/tracks", Method.GET);
                var result = await restClient.ExecuteGetTaskAsync(request);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    tracks = JsonConvert.DeserializeObject<List<Track>>(result.Content);
                }
            }

            listView.ItemsSource = tracks;

            listView.IsVisible = true;
            activityIndicator.IsVisible = false;
        }
    }
}
