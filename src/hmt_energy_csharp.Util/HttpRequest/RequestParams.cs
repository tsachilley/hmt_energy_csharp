using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.HttpRequest
{
    public class RequestParams
    {
        public Method Method { get; set; } = Method.Post;
        public string Route { get; set; }
        public string accept { get; set; } = "*/*";
        public string ContentType { get; set; } = "application/json";
        public Dictionary<string, string> Parameter { get; set; }
    }
}