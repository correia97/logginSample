using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly ILogger<ValuesController> _logger;
        private IHttpContextAccessor _accessor;
        public ValuesController(ILogger<ValuesController> logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _accessor = accessor;
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
                        _logger.Log(LogLevel.Information, line);
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
            _logger.Log(LogLevel.Information, "get with param {0}, {1}, {3}", id, ip, user);
            return "value";
        }





        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Pessoa
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
