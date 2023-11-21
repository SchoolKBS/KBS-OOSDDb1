namespace CampingCore
{
    public class Place
    {
        public int PlaceNumber { get; set; }
        public bool HasPower { get; set; }
        public int SurfaceArea { get; set; }
        public int PersonCount { get; set; }
        public double PricePerNight { get; set; }
        public string Description { get; set; }

        public Place(int placeNumber, bool hasPower, int surfaceArea, int numberOfPeople, double pricePerNight)
        {
            this.PlaceNumber = placeNumber;
            this.HasPower = hasPower;
            this.SurfaceArea = surfaceArea;
            this.PersonCount = numberOfPeople;
            this.PricePerNight = pricePerNight;
        }
        public override string ToString()
        {
            return "Plaats: " + PlaceNumber;
        }
    }
}
