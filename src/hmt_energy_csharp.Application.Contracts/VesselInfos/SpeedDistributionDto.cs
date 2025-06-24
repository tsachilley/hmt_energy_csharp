namespace hmt_energy_csharp.VesselInfos
{
    public class SpeedDistributionDto
    {
        public SpeedDistributionDto(double speed, int count)
        {
            Speed = speed;
            Count = count;
        }

        public double Speed { get; set; }
        public int Count { get; set; }
    }
}