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
        private readonly ICollection<dynamic> _data;

        public ZabbixController(ILogger<ZabbixController> logger, IConfiguration configuration, List<dynamic> data)
        {
            _logger = logger;
            _configuration = configuration;
            _data = data;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // var zabbix = new Zabbix.Zabbix(_configuration.GetValue<string>(UserZabixConfig),
            //     _configuration.GetValue<string>(PassZabbix),
            //     "https://opti.xpinc.io/zabbix-hml/api_jsonrpc.php");
            // await zabbix.LoginAsync();
            //
            // var response = await zabbix.GetResponseJsonAsync("event.get", new
            // {
            //     output = "extend",
            //     select_acknowledges = "extend",
            //     limit = 10,
            //     selectTags = "extend",
            //     selectSuppressionData = "extend",
            //     sortfield = new [] {"clock", "eventid"},
            //     sortorder = "DESC"
            // });
            // return Ok(response);

            return Ok(_data);
        }

        [HttpPost]
        public IActionResult Post([FromBody] dynamic data)
        {
            _data.Add(data);
            return Ok(data);
        }
    }
}