using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.StaticEntities
{
    public class ProtocolParam
    {
        public string number { get; set; } = "";
        public List<string> sentences { get; set; } = new List<string>();
        public string leftChars { get; set; } = "";
    }
}