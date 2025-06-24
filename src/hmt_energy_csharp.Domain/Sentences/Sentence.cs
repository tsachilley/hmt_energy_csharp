using hmt_energy_csharp.ConnEntities;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Sentences
{
    [Index(nameof(time), nameof(vdr_id), nameof(category), IsUnique = false, Name = "UK_Sentence_TVC")]
    public class Sentence : ConnEntity
    {
        public string data { get; set; }
        public long time { get; set; }
        public string vdr_id { get; set; }
        /*public string sentenceid { get; set; }
        public int? isdecoded { get; set; }*/
        public string category { get; set; }

        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;
    }
}