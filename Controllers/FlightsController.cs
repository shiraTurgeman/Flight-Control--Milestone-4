using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlightsControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        //the variables for the class.
        IMemoryCache cache;
        private IFlightManager FlightManager = new FlightManager();

        //the constroctor of the class.
        public FlightsController(IMemoryCache myCache)
        {
            cache = myCache;
        }

        // GET: api/Flights/5
        //[HttpGet("{relativeTo}", Name = "Get")]
        [HttpGet]
        public ActionResult<List<Flights>> Get(DateTimeOffset relative_to)
        {
            //List<Flights>
            try
            {

                FlightManager f = new FlightManager();
                string queryString = Request.QueryString.Value;
                string relativeTo = relative_to.ToString();

                return f.getFlightsList(cache, queryString, relativeTo);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }


        }


        // POST: api/Flights
        //add a flight to the programm.
        [HttpPost]
        public void Post(Flights f)
        {
            FlightManager.AddFlight(f);
        }

        // PUT: api/Flights/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string Id)
        {
            try
            {
                FlightPlanManager f = new FlightPlanManager();
                f.deleteFlightPlan(cache, Id);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
