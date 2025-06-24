using System.Collections.Generic;

namespace hmt_energy_csharp.AnalyseStaticDatas
{
    public static class AnalyseStaticData
    {
        public static IDictionary<string, ByteRingBuffer> AnalyseRingBuffers = new Dictionary<string, ByteRingBuffer>();
    }
}