using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourSolutionName
{
    public static class Constants
    {
        public const string BaseURL = "http://api.openweathermap.org/data/2.5/forecast?";
        public const string AppID = "YourOpenWeatherMapID";
        public const string CityID = "YourCityID from OpenWeatherMap";

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
    }
}
