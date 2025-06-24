namespace hmt_energy_csharp.VesselInfos
{
    public class PowerDistributionDto
    {
        public PowerDistributionDto(double power, int count)
        {
            Power = power;
            Count = count;
        }

        public double Power { get; set; }
        public int Count { get; set; }
    }
}