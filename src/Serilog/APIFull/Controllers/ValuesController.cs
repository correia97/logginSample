using LogSample.Model;
using LogSample.Model.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace APIFull.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly ILogger<ValuesController> _logger;
        private IHttpContextAccessor _accessor;
        private IUserService _userService;
        public ValuesController(ILogger<ValuesController> logger, IHttpContextAccessor accessor, IUserService userService)
        {
            _logger = logger;
            _accessor = accessor;
            _userService = userService;

        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            //using (var file = new FileStream("SacramentocrimeJanuary2006.csv", FileMode.Open))
            //{
            //    using (var reader = new StreamReader(file))
            //    {                    
            //        string line;
            //        LogContext.PushProperty("AuditLog", true);
            //        while ((line = await reader.ReadLineAsync()) != null)
            //        {
            //            _logger.Log(LogLevel.Warning, line);
            //        }
            //    }
            //}
            var item = _userService.GetUser(Guid.NewGuid());
            var log = new LogModel();
            log.User = "Current User";
            log.OldData = JsonConvert.SerializeObject(item);

            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            log.NewData = JsonConvert.SerializeObject(item);


            LogContext.PushProperty("AuditLog", true);
            _logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));
            //_logger.Log(LogLevel.Information, "");
            return Ok(new string[] { "value1", "value2" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
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
}
