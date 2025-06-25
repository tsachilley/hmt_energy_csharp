namespace hmt_energy_csharp.Dtos
{
    public class BaseShipInfo
    {
        public int shipId { get; set; }
        public string number { get; set; } = "SAD1";
        public string ShipType { get; set; }
        public float? DWT { get; set; }
        public float? GT { get; set; }

        public string CurrentFuelType { get; set; }

        public double? Pitch { get; set; }
    }
}