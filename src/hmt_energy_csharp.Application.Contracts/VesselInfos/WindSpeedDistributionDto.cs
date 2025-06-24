namespace hmt_energy_csharp.VesselInfos
{
    public class WindSpeedDistributionDto
    {
        public WindSpeedDistributionDto(double windSpeed, int count)
        {
            WindSpeed = windSpeed;
            Count = count;
        }

        public double WindSpeed { get; set; }
        public int Count { get; set; }
    }
}