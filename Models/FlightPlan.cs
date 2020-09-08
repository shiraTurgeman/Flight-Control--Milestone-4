using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    //define all the featurs the flight has
    public struct initial_location
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string date_time { get; set; }
        public initial_location(double longitude, double latitude, string date_time)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.date_time = date_time;
        }
    }
    //the segments of the flights.
    public struct segment
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int timespan_seconds { get; set; }
        public String endTime { get; set; }

        public segment(String endTime, double longitude, double latitude, int timespan_seconds)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.timespan_seconds = timespan_seconds;
            this.endTime = endTime;
        }
    }
    public class FlightPlan
    {
        public List<segment> segmentsList = new List<segment>();
        private List<segment> _settings;

        public List<segment> segments
        {
            get { return _settings; }
            set { _settings = segmentsList; }
        }


        public int passengers { get; set; }
        public string company_name { get; set; }
        public bool is_external { get; set; }

        public initial_location loc { get; set; }
        public segment seg { get; set; }
        // public segment seg;
    }
}
