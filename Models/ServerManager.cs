using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class ServerManager : IServerManager
    {
        public void deleteServerById(IMemoryCache cache, string Id)
        {
            Dictionary<string, Server> ServersDictionary;
            //delete this server from the list.
            if (cache.TryGetValue("ServersDictionary", out ServersDictionary))
            {
                Server serverObject;
                if (ServersDictionary.TryGetValue(Id, out serverObject))
                {
                    ServersDictionary.Remove(Id);
                }
            }
        }
        //get the json from the sever and create an flight object.
        public void getJson(JsonElement value)
        {
            string s = System.Text.Json.JsonSerializer.Serialize(value);
            JObject data = JObject.Parse(s);
            JToken jToken = data;
            Server serverObject = new Server();
            serverObject.ServerId = (string)jToken["ServerId"];
            serverObject.ServerURL = (string)jToken["ServerURL"];

            //save the server in the servers list.
            AddToCache(serverObject);
        }

        //add this server to cache.
        public void AddToCache(Server serverObject)
        {
            string ID = serverObject.ServerId;
            ServersListId.Add(ID);
            ServersDictionary.Add(ID, serverObject);
        }

        //create the dictionary to save the id of the server.
        public static Dictionary<string, Server> ServersDictionary = new Dictionary<string, Server>();

        //get the server dictionary.
        public Dictionary<string, Server> getServersDictionary()
        {
            return ServersDictionary;
        }

        //create a list of the server's id.
        public static List<string> ServersListId = new List<string>();
        public List<string> getServersListId()
        {
            return ServersListId;
        }

        //create a dictionary between the server and its id.
        public static Dictionary<string, string> IdFromServer = new Dictionary<string, string>();

        public Dictionary<string, string> getIdFromServer()
        {
            return IdFromServer;
        }

        //return the server list.
        public List<Server> createServersList(Dictionary<string, Server> myServersDictionary)
        {
            List<Server> externalServersList = new List<Server>();
            Dictionary<string, Server> ServersDictionary = myServersDictionary;
            int i;
            for (i = 0; i < ServersListId.Count; i++)
            {
                string Id = ServersListId[i];
                Server serverObject;

                //check if this server's id is in the dictionary, if so- add it to the server list.
                if (ServersDictionary.TryGetValue(Id, out serverObject))
                {
                    externalServersList.Add(serverObject);
                }
            }

            return externalServersList;
        }

        //return a list of all the external flights.
        public List<Flights> externalFlightList(string relativeTo)
        {

            List<Flights> externalFlights = new List<Flights>(); //all Flights from all servers
            List<Flights> FlightListFromServer = new List<Flights>();

            int i;
            //iterate over all the server and return a list of all the flights.
            for (i = 0; i < ServersListId.Count; i++)
            {
                string Id = ServersListId[i];
                Server serverObject;

                if (ServersDictionary.TryGetValue(Id, out serverObject))
                {
                    FlightListFromServer = getFlightFromExternalServer(serverObject, relativeTo);
                }

                int j;
                //return a list of all the external flights from all of the external server.
                for (j = 0; j < FlightListFromServer.Count; j++)
                {
                    externalFlights.Add(FlightListFromServer[j]);
                }
            }
            return externalFlights;
        }

        //connect to a specific server and get its flight list.
        public List<Flights> getFlightFromExternalServer(Server server, string relativeTo)
        {
            //create the list
            List<Flights> FlightListFromServer = new List<Flights>();
            //create the server url
            string url = createUrl(server.ServerURL, relativeTo.Substring(0, 19));
            //connect to server
            string strresulttest = connectToExternalServer(url);
            //create the flights of this server
            //try {
            FlightListFromServer = createFlightListFromJson(strresulttest, server.ServerId);

            //}
            //catch () { }


            return FlightListFromServer;
        }

        //connect to the server.
        public string connectToExternalServer(string url)
        {
            string strurltest = String.Format(url);
            WebRequest requestObjeGet = WebRequest.Create(strurltest);
            requestObjeGet.Method = "GET";
            HttpWebResponse responseObjGet = null;
            responseObjGet = (HttpWebResponse)requestObjeGet.GetResponse();

            string strresulttest = null;
            using (Stream stream = responseObjGet.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                strresulttest = sr.ReadToEnd();
                sr.Close();
            }

            return strresulttest;
        }

        //create a lists of flight of an external server.
        public List<Flights> createFlightListFromJson(string strresulttest, string serverId)
        {

            List<Flights> externalFlightList = new List<Flights>();
            List<Flights> jsonArray = JsonConvert.DeserializeObject<List<Flights>>(strresulttest);

            int i;

            for (i = 0; i < jsonArray.Count; i++)
            {
                Flights FlightObject = new Flights();
                //create new flight.
                FlightObject.Flight_id = jsonArray[i].Flight_id;
                FlightObject.longitude = jsonArray[i].longitude;
                FlightObject.latitude = jsonArray[i].latitude;
                FlightObject.passengers = jsonArray[i].passengers;
                FlightObject.company_name = jsonArray[i].company_name;
                FlightObject.date_time = jsonArray[i].date_time;
                FlightObject.is_external = true;
                externalFlightList.Add(FlightObject);

                //check if the id isnt exist in the dictionary, if not insert 
                //the is and ther server to the list
                if (!IdFromServer.ContainsKey(FlightObject.Flight_id))
                {
                    IdFromServer.Add(FlightObject.Flight_id, serverId);
                }

            }

            return externalFlightList;
        }

        //create the right url for the server.
        public string createUrl(string serverUrl, string relativeTo)
        {
            int day = Int32.Parse(relativeTo.Substring(0, 2));
            int mounth = Int32.Parse(relativeTo.Substring(3, 2));
            int year = Int32.Parse(relativeTo.Substring(6, 4));
            int hour = Int32.Parse(relativeTo.Substring(11, 2));
            int min = Int32.Parse(relativeTo.Substring(14, 2));
            int sec = Int32.Parse(relativeTo.Substring(17, 2));
            //by format- "relative_to=year + "-" + mounth + "-" + day + "T" + hour + ":" + min + ":" + sec + "Z"
            string time = string.Format("{0:D4}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:{5:D2}Z",
                            year, mounth, day, hour, min, sec);
            string url = serverUrl + "/api/Flights?relative_to=" + time;

            return url;
        }

        //exept flight plan from external server by id.
        public FlightPlan externalFlightPlanById(string Id, string serverId)
        {
            Dictionary<string, Server> ServersDictionary = getServersDictionary();
            FlightPlan FlightPlanObject;

            Server serverObject;
            if (ServersDictionary.TryGetValue(serverId, out serverObject))
            {
                string url = serverObject.ServerURL + "/api/FlightPlan/" + Id;
                string strFlightPlan = connectToExternalServer(url);

                FlightPlanManager FlightPlanManager = new FlightPlanManager();

                FlightPlanObject = createFlightPlan(strFlightPlan);

                return FlightPlanObject;
            }
            else
            {
                //error
            }
            return null;

        }

        //create flight plan from json.
        public FlightPlan createFlightPlan(string value)
        {
            FlightPlanManager FlightPlanManager = new FlightPlanManager();
            JObject data = JObject.Parse(value);

            JToken jToken = data;
            FlightPlan FlightPlanObject = new FlightPlan();
            FlightPlanObject.passengers = (int)jToken["passengers"];
            FlightPlanObject.company_name = (string)jToken["company_name"];
            //parse "initial_location" property
            Object initialLocation = (Object)jToken["initial_location"];
            JObject dataInitialLocation = JObject.Parse(initialLocation.ToString());
            JToken jTokenLoc = dataInitialLocation;
            double longitude = (double)jTokenLoc["longitude"];
            double latitude = (double)jTokenLoc["latitude"];
            string date_time = (string)jTokenLoc["date_time"];
            FlightPlanObject.loc = new initial_location(longitude, latitude, date_time);
            //parse "segments" property
            Object segmentObj = (Object)jToken["segments"];
            JArray jsonArray = JArray.Parse(segmentObj.ToString());
            int i;
            dynamic segmentData;
            for (i = 0; i < jsonArray.Count; i++)
            {
                segmentData = JObject.Parse(jsonArray[i].ToString());
                longitude = (double)segmentData["longitude"];
                latitude = (double)segmentData["latitude"];
                int timespan_seconds = (int)segmentData["timespan_seconds"];
                string segmentEndTime = FlightPlanManager.TimeCalculate(date_time, i, timespan_seconds, FlightPlanObject);
                FlightPlanObject.segmentsList.Add(new segment(segmentEndTime, longitude, latitude, timespan_seconds));
            }
            FlightPlanObject.segments = FlightPlanObject.segmentsList;

            return FlightPlanObject;

        }
    }
}
