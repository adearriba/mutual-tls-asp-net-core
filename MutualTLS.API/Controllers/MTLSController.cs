using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MutualTLS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MTLSController : ControllerBase
    {
        private readonly ILogger<MTLSController> _logger;

        public MTLSController(ILogger<MTLSController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var claims = HttpContext.User.Claims
                .Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value)); 

            return new JsonResult(new
            {
                identity = HttpContext.User.Identity,
                claims = claims
            });
        }
    }
}
