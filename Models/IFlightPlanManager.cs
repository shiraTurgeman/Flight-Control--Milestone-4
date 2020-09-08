using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightPlanManager
    {
        //return FlightPlan by Id
        FlightPlan getFlightPlanById(IMemoryCache cache, string Id);
        //delete flightPlan from cache
        void deleteFlightPlan(IMemoryCache cache, string Id);

        //create a flight plan from json element and insert to cache.
        void getJson(JsonElement value);

        //create the flight plan.
        FlightPlan createFlightPlanFronJson(JsonElement value);

        //add the flight plan to cache.
        void AddToCache(FlightPlan FlightPlanObject);

        //return the list of the flight id.
        List<string> getFlightPlanListId();

        //delete the id from the list.
        void deleteId(string id);
        //return the dictionary of the flight plan.
        Dictionary<string, FlightPlan> getFlightPlanDictionary();

        //add flight plan to the list.
        void AddFlightPlan(FlightPlan f);
        //return all the flights.
        IEnumerable<FlightPlan> GetAllFlights();

        //create a random id for each flight plan.
        string CreateId();
        // This method returns a random lowercase letter.
        // ... Between 'a' and 'z' inclusize.
        char GetLetter();

        //create a time format.
        string parseTime(string relativeTo);

        //calculate the arrival time of the flight.
        string TimeCalculate(string dataTime, int i, int timespanSecond, FlightPlan FlightPlanObject);

    }
}
