using hmt_energy_csharp.ConnEntities;
using System.ComponentModel.DataAnnotations;

namespace hmt_energy_csharp.Sentences
{
    public class CreateSentenceDto : ConnEntityDto
    {
        [Required]
        [StringLength(1024)]
        public string data { get; set; }

        [Required]
        public long time { get; set; }

        [Required]
        public string vdr_id { get; set; }

        /*[Required]
        public string sentenceid { get; set; }
        [Required]
        public int? isdecoded { get; set; }*/

        [Required]
        public string category { get; set; }
    }
}