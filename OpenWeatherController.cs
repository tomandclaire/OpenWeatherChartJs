using ChartJSCore.Models;
using YourSolutionName.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YourSolutionName.Controllers
{

    public class OpenWeatherController : Controller
    {        
        public ActionResult Index()
        {
            OpenWeatherMap localWeather = GetLocalWeather();
            return View(localWeather);
        }

        public static string DegreesToCardinal(double degrees)
        {
            string[] caridnals = { "Northerly", "NorthEasterly", "Easterly", "SouthEasterly", "Southerly", "SouthWesterly", "Westerly", "NorthWesterly", "Northerly" };
            return caridnals[(int)Math.Round(((double)degrees % 360) / 45)];
        }

        public static string DegreesToCardinalDetailed(double degrees)
        {
            degrees *= 10;

            string[] caridnals = { "North", "NNE", "NE", "ENE", "East", "ESE", "SE", "SSE", "South", "SSW", "SW", "WSW", "West", "WNW", "NW", "NNW", "North" };
            return caridnals[(int)Math.Round(((double)degrees % 3600) / 225)];
        }

        public static OpenWeatherMap GetLocalWeather()
        {
            string apiUrl = Constants.BaseURL + "id=" + Constants.CityID + "&appid=" + Constants.AppID + "&units=metric";

            string result = RequestHandler.Process(apiUrl);

            OpenWeatherMap lWeather = JsonConvert.DeserializeObject<OpenWeatherMap>(result);

            return lWeather;
        }

        public static List<TempData> GetTempForecast()
        {
            List<TempData> temperatures = new List<Models.TempData>();

            OpenWeatherMap lWeather = GetLocalWeather();

            foreach (var item in lWeather.list)
            {
                temperatures.Add(new Models.TempData
                {
                    dt = DateTime.Parse(item.dt_txt),                  
                    temperatureC = (Math.Round(item.main.temp)),
                    humidity = (double)item.main.humidity
                });
            }
            return temperatures;
        }

        private static List<WindData> GetWindForecast()
        {
            List<WindData> windSpeed = new List<WindData>();
            OpenWeatherMap lWeather = GetLocalWeather();

            foreach (var item in lWeather.list)
            {
                windSpeed.Add(new Models.WindData
                {
                    dt = DateTime.Parse(item.dt_txt),
                    windSpeed = (double)(item.wind.speed) * 3.6,
                    windDir = (double)item.wind.deg,
                    cloudCover = (int)item.clouds.all
                }); ;
            }
            return windSpeed;
        }

        public IActionResult Graphs()
        {
            Chart tempChart = GenerateTempChart();
            Chart humidityChart = GenerateHumidityChart();
            Chart windChart = GenerateWindChart();
            Chart cloudChart = GenerateCloudChart();

            ViewData["TemperatureChart"] = tempChart;
            ViewData["HumidityChart"] = humidityChart;
            ViewData["WindChart"] = windChart;
            ViewData["CloudChart"] = cloudChart;

            return View();
        }
        

    private static Chart GenerateTempChart()
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            List<DateTime> labels = GetTempForecast().Select(e => e.dt).ToList();
            
            List<string> shortLabels = labels.ConvertAll(x => x.ToString("ddd HH mm"));
            
            data.Labels = shortLabels;
            

            LineDataset dataset = new LineDataset()
            {
                Label = "Temperature",                
                Data = GetTempForecast().Select(e => e.temperatureC).ToList(),
                LineTension = 0.5,
                BackgroundColor = "rgba(75, 192, 192, 0)",
                BorderColor = "rgba(244, 66, 66, 1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(244, 66, 66, 1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 3 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(244, 66, 66, 1)" },
                PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };
                      
            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            
            Options options = new Options()
            {
               Scales = new Scales(),

               Legend = new Legend()
               {
                   Labels = new LegendLabel()
                   {
                       BoxWidth = 0,
                       FontSize = 18
                   }
               }                                                                          
            };

            Scales scale = new Scales()
            {
                
            };

            TimeScale xAxes = new TimeScale()
            {                                               
                Ticks = new Tick()                          
            };

            Tick tick = new Tick()
            {
                FontColor = "rgba(244, 66, 66, 1)"
            };

            
            CartesianScale yAxes = new CartesianScale()
            {
                Ticks = new CartesianLinearTick()
                {
                    Max = 50,
                    BeginAtZero = true,
                    Callback = "function(value, index, values) {return value + ' °C' }"
                }
                
            };

            xAxes.Ticks = tick;
            scale.YAxes = new List<Scale>() { yAxes };
            scale.XAxes = new List<Scale>() { xAxes };
            
            options.Scales = scale;
            chart.Options = options;

            chart.Data = data;

            return chart;
        }

        private static Chart GenerateHumidityChart()
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            List<DateTime> labels = GetTempForecast().Select(e => e.dt).ToList();

            List<string> shortLabels = labels.ConvertAll(x => x.ToString("ddd HH mm"));

            data.Labels = shortLabels;

            LineDataset dataset = new LineDataset()
            {
                Label = "Humidity",
                Data = GetTempForecast().Select(e => e.humidity).ToList(),
                LineTension = 0.5,
                BackgroundColor = "rgba(75, 192, 192, 0)",
                BorderColor = "rgba(244, 194, 66, 1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(244, 194, 66, 1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 3 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(244, 194, 66, 1)" },
                PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            Options options = new Options()
            {
                Legend = new Legend()
                {
                    Labels = new LegendLabel()
                    {
                        BoxWidth = 0,
                        FontSize = 18
                    }
                }
            };
            CartesianScale xAxes = new CartesianScale()
            {
                Ticks = new CartesianLinearTick()
                {
                    FontColor = "rgba(244, 194, 66, 1)"
                }
            };
            Scales scale = new Scales()
            {
                YAxes = new List<Scale>()
                {
                    new CartesianScale()
                    {
                        Ticks = new CartesianLinearTick()
                        {
                            Max = 100,
                            BeginAtZero = true,
                            Callback = "function(value, index, values) {return value + ' %' }"
                        }
                    }
                }
            };
            options.Scales = scale;
            chart.Options = options;

            
            scale.XAxes = new List<Scale>() { xAxes };

            return chart;
        }


        private static Chart GenerateWindChart()
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            List<DateTime> labels = GetWindForecast().Select(e => e.dt).ToList();

            List<string> shortLabels = labels.ConvertAll(x => x.ToString("ddd HH mm"));

            data.Labels = shortLabels;

            

            LineDataset dataset = new LineDataset()
            {
                Label = "Windspeed",
                Data = GetWindForecast().Select(e => e.windSpeed).ToList(),
                LineTension = 0.5,
                BackgroundColor = "rgba(75, 192, 192, 0)",
                BorderColor = "rgba(69, 66, 244, 1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(69, 66, 244, 1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 3 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(69, 66, 244, 1)" },
                PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            Options options = new Options()
            {
                Legend = new Legend()
                {
                    Labels = new LegendLabel()
                    {
                        BoxWidth = 0,
                        FontSize = 18
                    }
                }
            };
            Scales scale = new Scales()
            {
                YAxes = new List<Scale>()
                {
                    new CartesianScale()
                    {
                        Ticks = new CartesianLinearTick()
                        {
                            Max = 70,
                            BeginAtZero = true,
                            Callback = "function(value, index, values) {return value + ' kph' }"
                        }
                    }
                }
            };
            CartesianScale xAxes = new CartesianScale()
            {
                Ticks = new CartesianLinearTick()
                {
                    FontColor = "rgba(69, 66, 244, 1)"
                }
            };
            options.Scales = scale;
            chart.Options = options;
            scale.XAxes = new List<Scale>() { xAxes };

            return chart;
        }

        private static Chart GenerateCloudChart()   
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            List<DateTime> labels = GetWindForecast().Select(e => e.dt).ToList();

            List<string> shortLabels = labels.ConvertAll(x => x.ToString("ddd HH mm"));

            data.Labels = shortLabels;


            LineDataset dataset = new LineDataset()
            {
                Label = "Cloud cover",
                Data = GetWindForecast().Select(e => e.cloudCover).ToList(),
                LineTension = 0.5,
                BackgroundColor = "rgba(75, 192, 192, 0)",
                BorderColor = "rgba(75,192,192,1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(75,192,192,1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 3 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(75,192,192,1)" },
                PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            Options options = new Options()
            {
                Legend = new Legend()
                {
                    Labels = new LegendLabel()
                    {
                        BoxWidth = 0,
                        FontSize = 18
                    }
                }
            };
            Scales scale = new Scales()
            {
                YAxes = new List<Scale>()
                {
                    new CartesianScale()
                    {
                        Ticks = new CartesianLinearTick()
                        {
                            Max = 100,
                            BeginAtZero = true,
                            Callback = "function(value, index, values) {return value + ' %' }"
                        }
                    }
                }
            };
            CartesianScale xAxes = new CartesianScale()
            {
                Ticks = new CartesianLinearTick()
                {
                    FontColor = "rgba(75,192,192,1)"
                }
            };
            options.Scales = scale;
            chart.Options = options;
            scale.XAxes = new List<Scale>() { xAxes };
            return chart;
        }


    }
   
}
