using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Weather
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public class WeatherData
        {
            public Current current { get; set; }
        }

        public class Current
        {
            public double temperature { get; set; }
            public string weather_code { get; set; }
            public List<string> weather_icons { get; set; }
            public List<string> weather_descriptions { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        [Obsolete]
        public void OnClickGoButton(object sender, EventArgs e)
        {
            var City = CityName.Text;
            var RC = new RestClient();
            var Request = new RestRequest("http://api.weatherstack.com/current?access_key=0f937552c1610e59df988defa5535e9e&query=" + City);

            RC.ExecuteAsyncGet(Request, (IRestResponse response, RestRequestAsyncHandle arg2) =>
            {
                var Data = JsonConvert.DeserializeObject<WeatherData>(response.Content);
                String ConditionFile;

                var icon = Data.current.weather_icons;
                //url к картинке теперь доуступен тут: icon[0]
                var desc = Data.current.weather_descriptions;

                switch (Data.current.weather_code)
                {
                    case "113":
                        ConditionFile = "sunny.png";
                        break;
                    case "119":
                        ConditionFile = "cloudy.png";
                        break;
                    default:
                        ConditionFile = "clouds.png";
                        break;
                }

                var Temp = (int)Data.current.temperature;
                var TempText = Temp.ToString();
                if (Temp > 0)
                {
                    TempText = "+" + Temp.ToString();
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Condition.Source = ConditionFile;
                    Temperature.Text = City.ToString() + " : " + TempText + ", " + desc[0];
                    CityName.Text = "";
                    CityName.IsEnabled = true;
                });
            }, "GET");

            CityName.Text = "Загрузка..."; //код ассинхр, поэтому эта часть выполниться раньше чем запрос на сервер
            CityName.IsEnabled = false;
        }
    }
}