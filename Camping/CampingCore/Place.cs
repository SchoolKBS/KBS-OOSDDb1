using System.Collections;
using System.IO;

namespace CampingCore
{
    public class Place
    {
        public int PlaceID { get; set; }
        public bool Power { get; set; }
        public int StreetID { get; set; }
        public int AreaID { get; set; }
        public bool Dogs { get; set; }
        public int SurfaceArea { get; set; }
        public int AmountOfPeople { get; set; }
        public double PricePerNightPerPerson { get; set; }
        public int XCord { get; set; }
        public int YCord { get; set; }

        public Place(int placeID, bool hasPowerbool, int streetID, int areaID, bool canHaveDogs, int surfaceArea, int numberOfPeople, double pricePerNightPerPerson, int xCord, int yCord)
        {
            this.PlaceID = placeID;
            this.Power = hasPowerbool;
            this.StreetID = streetID;
            this.AreaID = areaID;
            this.SurfaceArea = surfaceArea;
            this.Dogs = canHaveDogs;
            this.AmountOfPeople = numberOfPeople;
            this.PricePerNightPerPerson = pricePerNightPerPerson;
            this.XCord = xCord;
            this.YCord = yCord;

        }

        public Place(ArrayList properties)
        {
            this.PlaceID = (int) properties[0];
            this.StreetID = (int)properties[1];
            this.AreaID = (int)properties[2];
            this.Power = (bool)properties[3];
            this.Dogs = (bool)properties[4];
            this.SurfaceArea = (int)properties[5];
            this.PricePerNightPerPerson = (double)properties[6];
            this.AmountOfPeople = (int)properties[7];
            this.XCord = (int)properties[8];
            this.YCord = (int)properties[9];
        }

        public int[] GetPlacePositions()
        {
            return new int[2] { XCord, YCord};
        }
        public override string ToString()
        {
            return "Plaats: " + PlaceID;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is Place)) return false;
            Place that = (Place)obj;
            return that.PlaceID == PlaceID;
        }
    }
}
