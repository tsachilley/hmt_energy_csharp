namespace hmt_energy_csharp.VesselInfos
{
    public class DraftDistributionDTO
    {
        public DraftDistributionDTO(double draft, int count)
        {
            Draft = draft;
            Count = count;
        }

        public double Draft { get; set; }
        public int Count { get; set; }
    }
}