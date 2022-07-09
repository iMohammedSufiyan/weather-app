using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace RestDemo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CityList : ContentPage
    {
        private Location location;
        private HttpClient client;
        private string citySearched;
        public ObservableCollection<WeatherDetailModel> weatherDetailModelCollection { get; set; }
        
        public CityList()
        {
            BindingContext = this;
            InitializeComponent();
            weatherDetailModelCollection = new ObservableCollection<WeatherDetailModel>();
            listView.ItemsSource = weatherDetailModelCollection;
            GetCurrentLocation();
        }

        // getting current location name on app startup..
        private async void GetCurrentLocation()
        {
            try
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest(
                    GeolocationAccuracy.High,
                    TimeSpan.FromSeconds(30) // search for only 30 seconds.
                ));

                var lat = location.Latitude;
                var lng = location.Longitude;

                var placemarks = await Geocoding.GetPlacemarksAsync(lat, lng);
                var placemark = placemarks?.FirstOrDefault();

                WeatherDetailModel weatherDetailModel = new WeatherDetailModel();
                weatherDetailModel.CityName = placemark.SubAdminArea.ToString();

                // calling this method to get weather info..
                GetWeatherInfo(lat, lng, weatherDetailModel);

                
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", fnsEx.Message, "Ok");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "Ok");
            }

        }

        // getting weather info from api..
        public async void GetWeatherInfo(double lat, double lng, WeatherDetailModel weatherDetailModel)
        {
            client = new HttpClient();

            var uri = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lng}&appid=2c408b01544fc60c60146d6b115e257b";

            // client.GetAsync(uri) get data from openweather api and return it to response.
            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                try
                {
                    var Items = JsonConvert.DeserializeObject<Rootobject>(content);
                    string s = Items.weather[0].description.ToString();
                    weatherDetailModel.Description = char.ToUpper(s[0]) + s.Substring(1);
                    weatherDetailModel.Temprature = ((int)(Items.main.temp - 273.15)).ToString();
                    weatherDetailModel.FeelsLike = "Feels Like : " + ((int)(Items.main.feels_like - 273.15)).ToString();
                    weatherDetailModel.Humidity = "Humidity : " + (Items.main.humidity).ToString();
                    weatherDetailModel.WindSpeed = String.Format("Wind Speed : {0:0.00}", (Items.wind.speed * 2.23694));

                    weatherDetailModelCollection.Add(weatherDetailModel);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "Ok");
                }
            }
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) 
                return;

            var weatherDetail = e.SelectedItem as WeatherDetailModel;
            await Navigation.PushAsync(new MainPage(weatherDetail));
            listView.SelectedItem = null;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            citySearched = e.NewTextValue;
        }

        // this method is called when user search for location or city..
        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync(citySearched);
                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    WeatherDetailModel weatherDetailModel = new WeatherDetailModel();

                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();
                    weatherDetailModel.CityName = placemark.SubAdminArea;
                    GetWeatherInfo(location.Latitude, location.Longitude, weatherDetailModel);

                    //await Navigation.PushAsync(new MainPage(weatherDetailModel), true);
                }
            }
            catch (FeatureNotEnabledException fnsEx)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", fnsEx.Message, "Ok");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "Ok");
            }
        }

    }
}