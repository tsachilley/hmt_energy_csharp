using hmt_energy_csharp.ConnEntities;

namespace hmt_energy_csharp.Sentences
{
    public class SentenceDto : ConnEntityDto
    {
        public long time { get; set; }
        public string data { get; set; }
        public string category { get; set; }
        public string vdr_id { get; set; }
        public byte Uploaded { get; set; } = 0;
    }
}