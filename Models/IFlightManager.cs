using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {

        List<Flights> getFlightsList(IMemoryCache cache, string queryString, string relativeTo);
        //check in which segments the flight is now.
        bool checkSegment(FlightPlan FlightPlan, TimeSpan start, TimeSpan end, TimeSpan now);

        //return a list of all the flights.
        IEnumerable<Flights> GetAllFlights();

        //create a list from the flights.
        List<Flights> createFlightList(Dictionary<string, FlightPlan> myFlightPlanDictionary,
            string relativeTo, bool syncAll, List<string> myFlightPlanListId);

        //check if the flight is still active and return the answer.
        bool activeFligth(FlightPlan FlightPlanObject, string relativeTo);

        //return the current longtitude of the flight.
        //calculate the current longtitude of the flight according to the Proportional percentage
        //of the flight route so far.
        double calculateLong(int i, FlightPlan FlightPlan, string relativeTo);

        //return the current latitude of the flight.
        //calculate the current latitude of the flight according to the Proportional percentage
        //of the flight route so far.
        double calculateLat(int i, FlightPlan FlightPlan, string relativeTo);

        //create a flight object with all the fields.
        Flights createFlight(FlightPlan FlightPlan, string Id, string relativeTo);

        //check if we need to return external flight.
        bool checkSyncAll(string relativeTo);
        void AddFlight(Flights f);
    }
}
