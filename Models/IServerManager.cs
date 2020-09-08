using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IServerManager
    {
        //delete server
        void deleteServerById(IMemoryCache cache, string Id);
        //get the json from the sever and create an flight object.
        void getJson(JsonElement value);
        //add this server to cache.
        void AddToCache(Server serverObject);
        //get the server dictionary.
        Dictionary<string, Server> getServersDictionary();
        List<string> getServersListId();

        Dictionary<string, string> getIdFromServer();

        //return the server list.
        List<Server> createServersList(Dictionary<string, Server> myServersDictionary);

        //return a list of all the external flights.
        List<Flights> externalFlightList(string relativeTo);

        //connect to a specific server and get its flight list.
        List<Flights> getFlightFromExternalServer(Server server, string relativeTo);

        //connect to the server.
        string connectToExternalServer(string url);

        //create a lists of flight of an external server.
        List<Flights> createFlightListFromJson(string strresulttest, string serverId);

        //create the right url for the server.
        string createUrl(string serverUrl, string relativeTo);

        //exept flight plan from external server by id.
        FlightPlan externalFlightPlanById(string Id, string serverId);

        //create flight plan from json.
        FlightPlan createFlightPlan(string value);
    }
}
