﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flights
    {
        public string Flight_id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int passengers { get; set; }
        public string company_name { get; set; }
        public string date_time { get; set; }
        public bool is_external { get; set; }
    }
}
