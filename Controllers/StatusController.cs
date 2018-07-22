using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Galilei.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [Route("ServerStatus")]
        [HttpGet]
        public IActionResult ServerStatus()
        {
            return Ok(new {status = "green"});
        }
    }
}