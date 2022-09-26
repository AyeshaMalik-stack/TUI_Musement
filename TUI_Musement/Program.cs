using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;



namespace TUI_Musement
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string tui_Api_Url = "https://api.musement.com/api/v3/cities";
        //Get weather api key from weatherapi.com
        private static string weatherApiUrl = "https://api.weatherapi.com/v1/forecast.json?key=4ed08cc4d75b4c2db81165557222409&q=";


        static async Task Main(string[] args)
        {
            var allCities = (List<Cities>)null;
            // Get all cities where TUI Musement has activities to sell 
            try {
                allCities = await getCitiesFromTUIApi(); 
            }
             catch (Exception ex)
            {
                Console.WriteLine("Exception Message: " + ex.Message);
            }
        
         //Display weather information after getting list of cities
            if(allCities!=null)
            { 
            showWeather(allCities);
            }

        }
        // Return cities where TUI Musement has activities to sell
        private static async Task<List<Cities>> getCitiesFromTUIApi()
        {
            var cities =(List<Cities>) null;
            //Make HTTP requests to a web server, using async and await
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                //Get stream of all cities where TUI Musement has activities to sell
                var streamTask = client.GetStreamAsync(tui_Api_Url);
                //Deserializd the json responce
                cities = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Cities>>(await streamTask);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Message: " + ex.Message);
            }
            return cities;         
        }
        //Display weather information each city one by one
        private static void showWeather(List<Cities> cities)
        {
            if(cities!=null)
            { 
            foreach (var cityName in cities)
            {
                string weatherForecast = processWeatherForCity(cityName.Name);
                    //If weatherforecast dont get correct value than show error message and exit from loop
                    if(weatherForecast.Equals("unknown"))
                    {
                        Console.WriteLine("unable to process");
                        break;
                    }
                    //else if weather forecast get the required information than display the result
                    else
                    { 
                string[] splitResult = weatherForecast.Split('-');
                Console.WriteLine("Processed City:[" + cityName.Name + "]|[" + splitResult[0] + "]-[" + splitResult[1] + "]");
                    }
                }
            }
        }
        //Process weather information from WeatherAPI for each city available in TUI Musement's catalogue 
        private static string processWeatherForCity(string cityName)
        {
            string weatherApiUrlFull = weatherApiUrl+cityName+"&days=2";
            string weatherToday = "unknown";
            string weatherTOmorrow = "unknown";
            var client = new WebClient();
            try
            {
                //To get the forecast for a city 
                var content = client.DownloadString(weatherApiUrlFull);
                var jsonContent = JsonConvert.DeserializeObject<dynamic>(content);
                //Get current weather
                weatherToday = jsonContent.current.condition.text;
                foreach (var item in jsonContent.forecast.forecastday)
                {
                    //Get the weather condition for the next day
                    weatherTOmorrow = item.day.condition.text;
                }
                return weatherToday + "-" + weatherTOmorrow;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Message: " + ex.Message);
                return "unknown";
            }
           
        }
    }
}
