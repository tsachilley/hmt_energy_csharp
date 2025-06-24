namespace hmt_energy_csharp.VesselInfos
{
    public class SlipDistributionDto
    {
        public SlipDistributionDto(double slip, int count)
        {
            Slip = slip;
            Count = count;
        }

        public double Slip { get; set; }
        public int Count { get; set; }
    }
}