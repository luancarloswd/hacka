using System.Dynamic;

namespace Hacka.Zabbix.Models
{
    public class Response
    {
        public int id { get; set; }
        public string jsonrpc { get; set; }
        public dynamic result = new ExpandoObject();
        public Error error { get; set; }
    }
}