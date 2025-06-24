namespace hmt_energy_csharp.VesselInfos
{
    public class WindDirDistributionDto
    {
        public WindDirDistributionDto(double windDir, int count)
        {
            WindDir = windDir;
            Count = count;
        }

        public double WindDir { get; set; }
        public int Count { get; set; }
    }
}