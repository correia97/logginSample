using LogSample.Model;
using LogSample.Model.Enum;
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
        private readonly IElasticService<UserModel> _eService;

        public ValuesController(ILogger<ValuesController> logger,
            IHttpContextAccessor accessor,
            IUserService userService,
            IElasticService<UserModel> eService)
        {

            _logger = logger;
            _accessor = accessor;
            _userService = userService;
            _eService = eService;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {

            var item = _userService.GetUser(Guid.Parse("53ffa492-5e18-4733-97f7-4255a322ef41"));


            var logItem = new LogItem<UserModel>("CurrentUserName", ActionType.Update, item.Clone() as UserModel);
            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            logItem.SetNewData(item);


            LogContext.PushProperty("AuditLog", true);

            await _eService.RegisterOrUpdate(logItem, logItem.ObjectName, logItem.ObjectId);
            //_logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));


            return Ok();
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
