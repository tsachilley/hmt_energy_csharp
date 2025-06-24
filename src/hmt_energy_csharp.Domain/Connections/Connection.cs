using hmt_energy_csharp.ConnEntities;

namespace hmt_energy_csharp.Connections
{
    public class Connection : ConnEntity
    {
        public string number { get; set; }
        public string host { get; set; }
        public int? port { get; set; }
        public int? status { get; set; }
    }
}