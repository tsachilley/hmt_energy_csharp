using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Dtos
{
    public class TcpConfigParam
    {
        public bool IsReady { get; set; } = false;
        public string Content { get; set; }
    }
}