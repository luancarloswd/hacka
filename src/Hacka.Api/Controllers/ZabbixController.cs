using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hacka.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZabbixController : ControllerBase
    {
        private readonly ILogger<ZabbixController> _logger;
        private readonly IConfiguration _configuration;

        private const string UserZabixConfig = "UserZabbix";
        private const string PassZabbix = "PassZabbix";

        public ZabbixController(ILogger<ZabbixController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var zabbix = new Zabbix.Zabbix(_configuration.GetValue<string>(UserZabixConfig),
                _configuration.GetValue<string>(PassZabbix),
                "https://opti.xpinc.io/zabbix-hml/api_jsonrpc.php");
            await zabbix.LoginAsync();
            var response = await zabbix.GetResponseJsonAsync("action.get", new
            {
                output = "extend",
                selectOperations = "extend",
                selectRecoveryOperations = "extend",
                selectAcknowledgeOperations = "extend",
                selectFilter = "extend",
                filter = new
                {
                    eventsource = 0
                }
            });
            return Ok();
        }
    }
}