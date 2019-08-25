using LogSample.Model;
using LogSample.Model.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace APICore.Controllers
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
            ///Log.ForContext("AuditLog", true);
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
            var log = new LogModel<UserModel>("Current User");

            log.SetOldData(item.Clone() as UserModel);

            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            log.SetNewData(item);

            LogContext.PushProperty("AuditLog", true);
            _logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));
            //_logger.Log(LogLevel.Information, "");
            return Ok(new string[] { "value1", "value2" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var user = "user2@user.com";
            _logger.Log(LogLevel.Warning, "get with param {0}, {1}, {3}",id, ip, user);
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
