using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        //the variables for the class.
        IMemoryCache cache;
        private IFlightPlanManager FlightPlanManager = new FlightPlanManager();
        //the dictionary of the class, to hold all the flights.
        Dictionary<string, FlightPlan> FlightPlanDictionary;
        //the constroctor of the class.
        public FlightPlanController(IMemoryCache myCache)
        {
            cache = myCache;
        }

        // GET: api/FlightPlan/5
        //return a specifice flight plan for a flight.
        [HttpGet("{Id}", Name = "GetPlan")]
        public IActionResult Get(string Id)
        {
            try
            {
                FlightPlanManager f = new FlightPlanManager();
                return Ok(f.getFlightPlanById(cache, Id));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // POST: api/FlightPlan
        //insert element(value) to the cache.
        [HttpPost]
        public IActionResult Post(JsonElement value)
        {
            try
            {
                FlightPlanManager FlightPlanManager = new FlightPlanManager();
                FlightPlanManager.getJson(value);
                string s = System.Text.Json.JsonSerializer.Serialize(value);

                if (!cache.TryGetValue("FlightPlanDictionary", out FlightPlanDictionary))
                {
                    FlightPlanDictionary = FlightPlanManager.getFlightPlanDictionary();
                    cache.Set("FlightPlanDictionary", FlightPlanDictionary);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // PUT: api/FlightPlan/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        //delete a specifice flight from the cache.
        [HttpDelete("{id}")]
        public void Delete(string Id)
        {

        }
    }
}
