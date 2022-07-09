using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json;

namespace RestDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage(WeatherDetailModel weatherDetail)
        {
            if (weatherDetail == null)
                throw new ArgumentNullException();

            BindingContext = weatherDetail;

            InitializeComponent();
        }
        
    }
}
