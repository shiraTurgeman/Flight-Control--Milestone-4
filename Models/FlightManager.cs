using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;


namespace FlightControlWeb.Models
{
    public class FlightManager : IFlightManager
    {
        public List<Flights> getFlightsList(IMemoryCache cache, string queryString, string relativeTo)
        {

            //if those word exist in the request- return a list of a all the flights.
            bool syncAll = queryString.Contains("sync_all");

            List<Flights> FlightList;
            Dictionary<string, FlightPlan> FlightPlanDictionary;
            //check if the dictionary exist in cache- if so, return a list of all the flights
            if (cache.TryGetValue("FlightPlanDictionary", out FlightPlanDictionary))
            {
                FlightManager FlightManager = new FlightManager();
                FlightPlanManager flightPlanManager = new FlightPlanManager();

                List<string> FlightPlanListId = new List<string>();
                if (!cache.TryGetValue("FlightPlanListId", out FlightPlanListId))
                {
                    FlightPlanListId = flightPlanManager.getFlightPlanListId();
                    cache.Set("FlightPlanListId", FlightPlanListId);
                }
                FlightList = FlightManager.createFlightList(FlightPlanDictionary, relativeTo, syncAll, FlightPlanListId);
                return FlightList;
            }
            //else- return all external flights
            else if (syncAll)
            {
                ServerManager serverManager = new ServerManager();
                FlightList = serverManager.externalFlightList(relativeTo);
                return FlightList;
            }
            return null;
        }
        //check in which segments the flight is now.
        public bool checkSegment(FlightPlan FlightPlan, TimeSpan start, TimeSpan end, TimeSpan now)
        {
            //TimeSpan now = DateTime.Now.TimeOfDay;
            if (start <= end)
            {
                // start and stop times are in the same day
                if (now >= start && now <= end)
                {
                    // current time is between start and stop
                    return true;
                }
            }
            else
            {
                // start and stop times are in different days
                if (now >= start || now <= end)
                {
                    // current time is between start and stop
                    return true;
                }
            }
            return false;

        }

        //create a list of the flights.
        private static List<Flights> FlightList = new List<Flights>();

        //add flight to the list of flights.
        public void AddFlight(Flights f)
        {
            FlightList.Add(f);
        }



        //return a list of all the flights.
        public IEnumerable<Flights> GetAllFlights()
        {
            return FlightList;
        }

        //create a list from the flights.
        public List<Flights> createFlightList(Dictionary<string, FlightPlan> myFlightPlanDictionary, string relativeTo, bool syncAll, List<string> myFlightPlanListId)
        {
            //make new objects.
            List<Flights> FlightList = new List<Flights>();
            Dictionary<string, FlightPlan> FlightPlanDictionary = myFlightPlanDictionary;
            FlightPlanManager FlightPlanManager = new FlightPlanManager();
            // List<string> FlightPlanListId = FlightPlanManager.getFlightPlanListId();
            List<string> FlightPlanListId = myFlightPlanListId;
            int i;

            //check if to sync the flights.
            bool externalList = checkSyncAll(relativeTo);

            if (syncAll)
            {
                ServerManager serverManager = new ServerManager();
                List<Flights> externalFlights = serverManager.externalFlightList(relativeTo);
                int l;
                for (l = 0; l < externalFlights.Count; l++)
                {
                    FlightList.Add(externalFlights[l]);
                }
            }

            for (i = 0; i < FlightPlanListId.Count; i++)
            {
                string Id = FlightPlanListId[i];
                FlightPlan FlightPlanObject;
                //get a flight plan by id.
                if (!FlightPlanDictionary.TryGetValue(Id, out FlightPlanObject))
                {
                    continue;
                }

                Flights Flights;
                //if this flight isn't active- continue.
                if (!activeFligth(FlightPlanObject, relativeTo))
                {
                    continue;
                }
                //if this flight exist- create flight from flight plan.
                else
                {
                    Flights = createFlight(FlightPlanObject, Id, relativeTo);
                }
                //if this flight is external, and we need only internal flight- continue.
                if ((!syncAll) && (Flights.is_external == true))
                {
                    continue;
                }
                //if this flight is external and we need external flights- add it to the list.
                else
                {
                    FlightList.Add(Flights);
                }
            }
            //return the flight list.
            return FlightList;
        }

        //check if the flight is still active and return the answer.
        public bool activeFligth(FlightPlan FlightPlanObject, string relativeTo)
        {
            TimeSpan now = TimeSpan.Parse(relativeTo.Substring(11, 8));
            TimeSpan start = TimeSpan.Parse(FlightPlanObject.loc.date_time.Substring(11, 8));
            int numSegments = FlightPlanObject.segmentsList.Count;
            TimeSpan end = TimeSpan.Parse(FlightPlanObject.segmentsList[numSegments - 1].endTime.Substring(11, 8));

            if (start <= end)
            {
                // start and stop times are in the same day
                if (now >= start && now <= end)
                {
                    // current time is between start and stop
                    return true;
                }
            }
            else
            {
                // start and stop times are in different days
                if (now >= start || now <= end)
                {
                    // current time is between start and stop
                    return true;
                }
            }
            return false;
        }

        //return the current longtitude of the flight.
        //calculate the current longtitude of the flight according to the Proportional percentage
        //of the flight route so far.
        public double calculateLong(int i, FlightPlan FlightPlan, string relativeTo)
        {
            TimeSpan segmentInitialTime;
            if (i == 0)
            {
                segmentInitialTime = TimeSpan.Parse(FlightPlan.loc.date_time.Substring(11, 8));
            }
            else
            {
                segmentInitialTime = TimeSpan.Parse(FlightPlan.segmentsList[i - 1].endTime.Substring(11, 8));
            }
            TimeSpan now = TimeSpan.Parse(relativeTo.Substring(11, 8));
            TimeSpan current = now - segmentInitialTime;
            double secondCurrentTime = current.TotalSeconds;


            int timeSpandSecond = FlightPlan.segmentsList[i].timespan_seconds;

            double startLoc;
            if (i == 0)
            {
                startLoc = FlightPlan.loc.longitude;
            }
            else
            {
                startLoc = FlightPlan.segmentsList[i - 1].longitude;
            }
            double endLoc = FlightPlan.segmentsList[i].longitude;
            double currentLoc = (((endLoc - startLoc)) * (secondCurrentTime / timeSpandSecond)) + startLoc;

            return currentLoc;
        }

        //return the current latitude of the flight.
        //calculate the current latitude of the flight according to the Proportional percentage
        //of the flight route so far.
        public double calculateLat(int i, FlightPlan FlightPlan, string relativeTo)
        {
            //DataTime
            TimeSpan segmentInitialTime;
            if (i == 0)
            {
                segmentInitialTime = TimeSpan.Parse(FlightPlan.loc.date_time.Substring(11, 8));
            }
            else
            {
                segmentInitialTime = TimeSpan.Parse(FlightPlan.segmentsList[i - 1].endTime.Substring(11, 8));
            }
            TimeSpan now = TimeSpan.Parse(relativeTo.Substring(11, 8));
            TimeSpan current = now - segmentInitialTime;
            double secondCurrentTime = current.TotalSeconds;

            int timeSpandSecond = FlightPlan.segmentsList[i].timespan_seconds;

            double startLoc;
            if (i == 0)
            {
                startLoc = FlightPlan.loc.latitude;
            }
            else
            {
                startLoc = FlightPlan.segmentsList[i - 1].latitude;
            }
            double endLoc = FlightPlan.segmentsList[i].latitude;
            double currentLoc = (((endLoc - startLoc)) * (secondCurrentTime / timeSpandSecond)) + startLoc;

            return currentLoc;
        }

        //create a flight object with all the fields.
        public Flights createFlight(FlightPlan FlightPlan, string Id, string relativeTo)
        {
            bool externalList = checkSyncAll(relativeTo);
            Flights Flights = new Flights();
            int i;
            //check in which segment the flight is currently in.
            for (i = 0; i < FlightPlan.segmentsList.Count; i++)
            {
                TimeSpan start;
                if (i == 0)
                {
                    start = TimeSpan.Parse(FlightPlan.loc.date_time.Substring(11, 8));
                }
                else
                {
                    start = TimeSpan.Parse(FlightPlan.segmentsList[i - 1].endTime.Substring(11, 8));
                }
                TimeSpan end = TimeSpan.Parse(FlightPlan.segmentsList[i].endTime.Substring(11, 8));
                TimeSpan now = TimeSpan.Parse(relativeTo.Substring(11, 8));
                //check the current longtitude and latitide.
                if (checkSegment(FlightPlan, start, end, now))
                {


                    Flights.longitude = CheckRangeLong(i, FlightPlan, relativeTo);
                    Flights.latitude = CheckRangeLat(i, FlightPlan, relativeTo);
                    break;
                }
            }
            //add the rest of the fields.
            Flights.passengers = FlightPlan.passengers;
            Flights.company_name = FlightPlan.company_name;
            Flights.date_time = FlightPlan.loc.date_time;
            Flights.is_external = FlightPlan.is_external;
            Flights.Flight_id = Id;
            //return the flight.
            return Flights;
        }
        public double CheckRangeLong(int i, FlightPlan FlightPlan, string relativeTo)
        {
            double newLong = calculateLong(i, FlightPlan, relativeTo);
            double longitude;
            if (newLong > 180)
            {
                longitude = 180;
            }
            else if (newLong < -180)
            {
                longitude = -180;
            }
            else
            {
                longitude = newLong;
            }
            return longitude;
        }

        public double CheckRangeLat(int i, FlightPlan FlightPlan, string relativeTo)
        {
            double newLat = calculateLat(i, FlightPlan, relativeTo);
            double latitude;
            if (newLat > 180)
            {
                latitude = 180;
            }
            else if (newLat < -180)
            {
                latitude = -180;
            }
            else
            {
                latitude = newLat;
            }
            return latitude;
        }
        //check if we need to return external flight.
        public bool checkSyncAll(string relativeTo)
        {
            string sync = "sync_all";
            bool ret = relativeTo.Contains(sync);
            if (ret)
            {
                return true;
            }
            return false;
        }
    }
}
