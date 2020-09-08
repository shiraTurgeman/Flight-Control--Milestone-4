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
    public class serversController : ControllerBase
    {
        //the variables for the class.
        IMemoryCache cache;

        //the constroctor of the class.
        public serversController(IMemoryCache myCache)
        {
            cache = myCache;
        }

        // GET: api/servers
        [HttpGet]
        public ActionResult<List<Server>> Get()
        {
            try
            {
                List<Server> externalServersList;
                Dictionary<string, Server> ServersDictionary;
                //return a list of all the server conncted to ours.
                if (cache.TryGetValue("ServersDictionary", out ServersDictionary))
                {
                    ServerManager serverManager = new ServerManager();
                    externalServersList = serverManager.createServersList(ServersDictionary);
                    return externalServersList;
                }
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        // GET: api/servers/5
        [HttpGet("{id}", Name = "GetServer")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/servers
        //add a server to the programm.
        [HttpPost]
        public IActionResult Post(JsonElement value)
        {
            try
            {
                Dictionary<string, Server> ServersDictionary;
                ServerManager serverManager = new ServerManager();

                serverManager.getJson(value);
                //if there is no dictionary- create a new one and save to cache.
                if (!cache.TryGetValue("ServersDictionary", out ServersDictionary))
                {
                    ServersDictionary = serverManager.getServersDictionary();
                    cache.Set("ServersDictionary", ServersDictionary);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        // PUT: api/servers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        //delete a server by id.
        [HttpDelete("{Id}")]
        public IActionResult Delete(string Id)
        {
            try
            {
                ServerManager s = new ServerManager();
                s.deleteServerById(cache, Id);
                return Ok();

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
