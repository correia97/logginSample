using LogSample.Model;
using LogSample.Model.Enum;
using LogSample.Model.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IMongoService<UserModel> _mongoService;

        public ValuesController(ILogger<ValuesController> logger,
            IHttpContextAccessor accessor,
            IUserService userService,
            IElasticService<UserModel> eService,
            IMongoService<UserModel> mongoService)
        {

            _logger = logger;
            _accessor = accessor;
            _userService = userService;
            _eService = eService;
            _mongoService = mongoService;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {

            var item = _userService.GetUser(Guid.NewGuid());


            var logItem = new LogItem<UserModel>("CurrentUserName", ActionType.Update, item.Clone() as UserModel);
            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            logItem.SetNewData(item);


            LogContext.PushProperty("AuditLog", true);

            await _eService.RegisterOrUpdate(logItem,  logItem.ObjectId);

            await _mongoService.RegisterOrUpdate(logItem,  logItem.ObjectId);
            //_logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));

            return Ok();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var item = _userService.GetUser(Guid.NewGuid());

            var logItem = new LogItem<UserModel>("CurrentUserName", ActionType.Update, item.Clone() as UserModel);
            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            logItem.SetNewData(item);

            LogContext.PushProperty("AuditLog", true);

            await _eService.RegisterOrUpdateNest(logItem,  logItem.ObjectId);
            await _mongoService.RegisterOrUpdate(logItem,  logItem.ObjectId);
            //_logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));

            return Ok();
        }

        // GET api/values/5
        [HttpGet]
        [Route("mongo")]
        public async Task<ActionResult> Mongo()
        {
            var item = _userService.GetUser(Guid.NewGuid());

            var logItem = new LogItem<UserModel>("CurrentUserName", ActionType.Update, item.Clone() as UserModel);
            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            logItem.SetNewData(item);

            LogContext.PushProperty("AuditLog", true);

            //await _eService.RegisterOrUpdateNest(logItem,  logItem.ObjectId);
            await _mongoService.RegisterOrUpdate(logItem,  logItem.ObjectId);
            //_logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));

            return Ok();
        }
        // GET api/values/5
        [HttpGet]
        [Route("elastichttp")]
        public async Task<ActionResult> ElasticHttp()
        {
            var item = _userService.GetUser(Guid.NewGuid());

            var logItem = new LogItem<UserModel>("CurrentUserName", ActionType.Update, item.Clone() as UserModel);
            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            logItem.SetNewData(item);

            LogContext.PushProperty("AuditLog", true);

            await _eService.RegisterOrUpdate(logItem,  logItem.ObjectId);
            // await _mongoService.RegisterOrUpdate(logItem,  logItem.ObjectId);
            //_logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));

            return Ok();
        }
        // GET api/values/5
        [HttpGet]
        [Route("elasticnest")]
        public async Task<ActionResult> ElasticNest()
        {
            var item = _userService.GetUser(Guid.NewGuid());

            var logItem = new LogItem<UserModel>("CurrentUserName", ActionType.Update, item.Clone() as UserModel);
            item.Email = "email2@email2.com";
            item.lastName = "Silva Sauro";

            _userService.Updateuser(item);
            logItem.SetNewData(item);

            LogContext.PushProperty("AuditLog", true);

            await _eService.RegisterOrUpdateNest(logItem,  logItem.ObjectId);
            // await _mongoService.RegisterOrUpdate(logItem,  logItem.ObjectId);
            //_logger.Log(LogLevel.Warning, JsonConvert.SerializeObject(log));

            return Ok();
        }


    }

    public class Pessoa
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
