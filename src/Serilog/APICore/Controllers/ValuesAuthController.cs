using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace APICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesAuthController : ControllerBase
    {
        readonly ILogger<ValuesAuthController> _logger;
        private IHttpContextAccessor _accessor;
        public ValuesAuthController(ILogger<ValuesAuthController> logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _accessor = accessor;
            var user = _accessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userName");
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            LogContext.PushProperty("UserName", user?.Value);
            LogContext.PushProperty("UserIp", ip);
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            using (var file = new FileStream("SacramentocrimeJanuary2006.csv", FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        _logger.Log(LogLevel.Warning, line);
                    }
                }
            }
            //_logger.Log(LogLevel.Information, "");
            return Ok(new string[] { "value1", "value2" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var user = "user2@user.com";
            _logger.Log(LogLevel.Warning, "get with param {0}, {1}, {3}", id, ip, user);
            return "value";
        }

        // POST: api/ValuesAuth
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/ValuesAuth/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
