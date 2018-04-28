using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using VollyV2.Data.Volly;

namespace VollyV2.Services
{
    public class GoogleLocator
    {
        public static string Endpoint = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        public static HttpClient Client = GetNewClient();

        private static HttpClient GetNewClient()
        {
            Client = new HttpClient();
            return Client;
        }

        public static Location GetLocationFromAddress(string addressString)
        {
            Client = GetNewClient();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string addressQuery = addressString.Replace(' ', '+');
            int tryCount = 0;
            do
            {
                HttpResponseMessage response = Client.GetAsync(Endpoint + addressQuery + "+Calgary").Result;

                var googleString = response.Content.ReadAsStringAsync().Result;
                GoogleLocation googleLocation = GoogleLocation.FromJson(googleString);
                List<LocationResult> results = googleLocation.Results;
                if (results.Count == 0)
                {
                    tryCount++;
                }
                else
                {
                    return new Location()
                    {
                        Latitude = results[0].Geometry.Location.Lat,
                        Longitude = results[0].Geometry.Location.Lng
                    };
                }
            } while (tryCount < 3);
            return null;
        }
    }
    public partial class GoogleLocation
    {
        [JsonProperty("results")]
        public List<LocationResult> Results { get; set; }
    }

    public class LocationResult
    {
        [JsonProperty("geometry")]
        public GoogleGeometry Geometry { get; set; }
    }

    public class GoogleGeometry
    {
        [JsonProperty("location")]
        public GoogleInnerLocation Location { get; set; }
    }

    public class GoogleInnerLocation
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public partial class GoogleLocation
    {
        public static GoogleLocation FromJson(string json) => JsonConvert.DeserializeObject<GoogleLocation>(json);
    }
}