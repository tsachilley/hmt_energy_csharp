using System.ComponentModel.DataAnnotations;

namespace hmt_energy_csharp.VDRs
{
    public class VdrTimeEntity
    {
        [Key]
        public string time { get; set; }
    }
}