using hmt_energy_csharp.ConnEntities;

namespace hmt_energy_csharp.VDRs
{
    public class VdrEntity : ConnEntity
    {
        public string sentenceid { get; set; }
        public string type { get; set; }
    }
}