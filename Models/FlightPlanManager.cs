using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace FlightControlWeb.Models
{
    public class FlightPlanManager : IFlightPlanManager
    {
        //create a dictionary of flight plan and id.
        public static Dictionary<string, FlightPlan> FlightPlanDictionary = new Dictionary<string, FlightPlan>();

        //return FlightPlan by Id
        public FlightPlan getFlightPlanById(IMemoryCache cache, string Id)
        {

            FlightPlan FlightPlan = null;
            IFlightManager FlightManager = new FlightManager();
            if (cache.TryGetValue("FlightPlanDictionary", out FlightPlanDictionary))
            {
                //Id = Id.Substring(1, Id.Length - 2);

                if (!FlightPlanDictionary.TryGetValue(Id, out FlightPlan))
                {
                    Console.WriteLine("error");
                }
            }

            ServerManager serverManager = new ServerManager();
            Dictionary<string, string> IdFromServer = serverManager.getIdFromServer();
            string serverId;
            if (Id[0] == '{')
            {
                Id = Id.Substring(1, Id.Length - 2);
            }
            if (IdFromServer.TryGetValue(Id, out serverId))
            {

                //get Flights plan from external server by id
                FlightPlan = serverManager.externalFlightPlanById(Id, serverId);
            }

            return FlightPlan;

        }

        public void deleteFlightPlan(IMemoryCache cache, string Id)
        {

            List<string> FlightPlanListId;
            if (cache.TryGetValue("FlightPlanListId", out FlightPlanListId))
            {
                int i = FlightPlanListId.IndexOf(Id);
                FlightPlanListId.RemoveAt(i);
            }



            Dictionary<string, FlightPlan> FlightPlanDictionary;
            //remove this flight from dictionary.
            if (cache.TryGetValue("FlightPlanDictionary", out FlightPlanDictionary))
            {
                FlightPlan FlightPlanObject;
                if (FlightPlanDictionary.TryGetValue(Id, out FlightPlanObject))
                {
                    FlightPlanDictionary.Remove(Id);
                }
            }
        }



        //create a flight plan from json element and insert to cache.
        public void getJson(JsonElement value)
        {
            FlightPlan FlightPlanObject = createFlightPlanFronJson(value);
            AddToCache(FlightPlanObject);
            AddFlightPlan(FlightPlanObject);
        }

        //create the flight plan.
        public FlightPlan createFlightPlanFronJson(JsonElement value)
        {
            string s = System.Text.Json.JsonSerializer.Serialize(value);
            JObject data = JObject.Parse(s);

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
                string segmentEndTime = TimeCalculate(date_time, i, timespan_seconds, FlightPlanObject);
                FlightPlanObject.segmentsList.Add(new segment(segmentEndTime, longitude, latitude, timespan_seconds));
            }
            FlightPlanObject.segments = FlightPlanObject.segmentsList;

            return FlightPlanObject;

        }

        //add the flight plan to cache.
        public void AddToCache(FlightPlan FlightPlanObject)
        {
            string ID = CreateId();
            FlightPlanListId.Add(ID);
            if (FlightPlanDictionary == null)
            {
                FlightPlanDictionary = new Dictionary<string, FlightPlan>();
                FlightPlanDictionary.Add(ID, FlightPlanObject);

            }
            else
            {
                FlightPlanDictionary.Add(ID, FlightPlanObject);
            }

        }

        //create a list for the flight plan.
        public static List<FlightPlan> FlightPlanList = new List<FlightPlan>();

        //create a list of the flight plans id.
        public static List<string> FlightPlanListId = new List<string>();

        //return the list of the flight id.
        public List<string> getFlightPlanListId()
        {
            return FlightPlanListId;
        }

        //delete the id from the list.
        public void deleteId(string id)
        {
            FlightPlanListId.Remove(id);
        }



        //return the dictionary of the flight plan.
        public Dictionary<string, FlightPlan> getFlightPlanDictionary()
        {
            return FlightPlanDictionary;
        }

        //add flight plan to the list.
        public void AddFlightPlan(FlightPlan f)
        {
            FlightPlanList.Add(f);
        }

        //return all the flights.
        public IEnumerable<FlightPlan> GetAllFlights()
        {
            return FlightPlanList;
        }

        //create a random id for each flight plan.
        public string CreateId()
        {
            string randomID = null;
            string num;
            char c;

            int i;
            //create four digits.
            for (i = 0; i < 4; i++)
            {
                num = new Random().Next(0, 10).ToString();
                randomID = randomID + num;
            }
            //create two letters.
            for (i = 0; i < 2; i++)
            {
                c = GetLetter();
                randomID = randomID + c;
            }
            //create four digits.
            for (i = 0; i < 4; i++)
            {
                num = new Random().Next(0, 10).ToString();
                randomID = randomID + num;
            }

            return randomID;
        }

        // This method returns a random lowercase letter.
        // ... Between 'a' and 'z' inclusize.
        public char GetLetter()
        {
            int num = new Random().Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let;
        }

        //create a time format.
        public string parseTime(string relativeTo)
        {

            int mounth = Int32.Parse(relativeTo.Substring(0, 2));
            int day = Int32.Parse(relativeTo.Substring(3, 2));
            int year = Int32.Parse(relativeTo.Substring(6, 4));

            int hour = Int32.Parse(relativeTo.Substring(11, 2));
            int min = Int32.Parse(relativeTo.Substring(14, 2));
            int sec = Int32.Parse(relativeTo.Substring(17, 2));

            string dateFormat = day.ToString() + " " + mounth.ToString() + " " + year.ToString() +
                " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
            return dateFormat;
        }

        //calculate the arrival time of the flight.
        public string TimeCalculate(string dataTime, int i, int timespanSecond, FlightPlan FlightPlanObject)
        {
            DateTime segmentInitialTime;
            DateTime segmentEndTime;
            if (i == 0)
            {
                string initialTime = parseTime(dataTime);
                segmentInitialTime = DateTime.Parse(initialTime);
            }
            else
            {
                segmentInitialTime = DateTime.Parse(FlightPlanObject.segmentsList[i - 1].endTime);
            }

            TimeSpan t = TimeSpan.FromSeconds(timespanSecond);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds,
                            t.Milliseconds);

            segmentEndTime = segmentInitialTime + TimeSpan.Parse(answer);

            return segmentEndTime.ToString();
        }
    }
}