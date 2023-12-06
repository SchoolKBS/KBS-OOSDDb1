using System.Collections;
using System.IO;

namespace CampingCore
{
    public class Place
    {
        public int PlaceID { get; set; }
        public bool Power { get; set; }
        public int StreetID { get; set; }
        public bool Dogs { get; set; }
        public int SurfaceArea { get; set; }
        public int AmountOfPeople { get; set; }
        public double PricePerNightPerPerson { get; set; }
        public int Xcord { get; set; }
        public int Ycord { get; set; }

        public Place(int placeID, bool hasPowerbool, int street, bool canHaveDogs, int surfaceArea, int numberOfPeople, double pricePerNightPerPerson, int xCord, int yCord)
        {
            this.PlaceID = placeID;
            this.Power = hasPowerbool;
            this.StreetID = street;
            this.SurfaceArea = surfaceArea;
            this.Dogs = canHaveDogs;
            this.AmountOfPeople = numberOfPeople;
            this.PricePerNightPerPerson = pricePerNightPerPerson;
            this.Xcord = xCord;
            this.Ycord = yCord;

        }
 /*       public Place(int placeID, bool hasPower, Street street, int surfaceArea, int numberOfPeople, double pricePerNightPerPerson, int xCord, int yCord) : this(placeID, hasPower, street, street.Dogs, surfaceArea, numberOfPeople, pricePerNightPerPerson, xCord, yCord)
        {

        }
        public Place(int placeID, Street street, bool canHaveDogs, int surfaceArea, int numberOfPeople, double pricePerNightPerPerson, int xCord, int yCord) : this(placeID, street.Power, street, canHaveDogs, surfaceArea, numberOfPeople, pricePerNightPerPerson, xCord, yCord)
        {

        }
        public Place(int placeID, Street street, int surfaceArea, int numberOfPeople, double pricePerNightPerPerson, int xCord, int yCord) : this(placeID, street.Power, street, street.Dogs, surfaceArea, numberOfPeople, pricePerNightPerPerson, xCord, yCord)
        {

        }*/
        public Place(ArrayList properties)
        {
            this.PlaceID = (int) properties[0];
            this.Power = (bool)properties[2];
            this.StreetID = (int)properties[1];
            this.SurfaceArea = (int)properties[3];
            this.PricePerNightPerPerson = (double)properties[4];
            this.AmountOfPeople = (int)properties[5];
            this.Dogs = (bool)properties[6];
            this.Xcord = (int)properties[7];
            this.Ycord = (int)properties[8];
        }

        public int[] GetPlacePositions()
        {
            // Xcord1 = width
            // Ycord1 = height
            return new int[2] { Xcord, Ycord};
        }
        public override string ToString()
        {
            return "Plaats: " + PlaceID;
        }
    }
}
