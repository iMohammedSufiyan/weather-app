using System;
using System.Collections.Generic;
using System.Text;

namespace RestDemo
{
    public class WeatherDetailModel
    {
        public string CityName { get; set; }
        public string Temprature { get; set; }
        public string Description { get; set; }
        public string FeelsLike { get; set; }
        public string Humidity { get; set; }
        public string WindSpeed { get; set; }
    }
}
