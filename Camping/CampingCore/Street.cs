using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Street : Facilities
    {
        public int StreetID { get; set; }
        public string Name { get; set; }
        public int XCord1 { get; set; }
        public int YCord1 { get; set; }
        public int XCord2 { get; set; }
        public int YCord2 { get; set; }

        public Street(ArrayList properties)
        {
            StreetID = (int)properties[0];
            Name = (string)properties[1];
            Power = (bool)properties[2];
            Dogs = (bool)properties[3];
            SurfaceArea = (int)properties[4];
            PricePerNightPerPerson = (double)properties[5];
            AmountOfPeople = (int)properties[6];
            XCord1 = (int)properties[7];
            YCord1 = (int)properties[8];
            XCord2 = (int)properties[9];
            YCord2 = (int)properties[10];
        }
        public Street(int streetID, string name, bool power, bool dogs, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, int Xcord1, int Ycord1, int Xcord2, int Ycord2)
        {
            StreetID = streetID;
            Name = name;
            Power = power;
            Dogs = dogs;
            SurfaceArea = surfaceArea; 
            PricePerNightPerPerson = pricePerNightPerPerson;
            AmountOfPeople = amountOfPeople;
            XCord1 = Xcord1;
            YCord1 = Ycord1;
            XCord2 = Xcord2;
            YCord2 = Ycord2;
        }

        public int[] GetStreetPositions()
        {
            return new int[4] { XCord1, YCord1, XCord2, YCord2 };
        }

        public override string ToString()
        {
            return base.ToString();
        }
        public static double CalcSideLenght(Street street, double textblockWidth, bool XSide)
        {
            double LineLenght = Math.Sqrt(Math.Pow(street.XCord2 - street.XCord1, 2) + Math.Pow(street.YCord1 - street.YCord2, 2));
            if (textblockWidth > LineLenght) return 0;
            double angle = Math.Atan2(street.XCord2 - street.XCord1, street.YCord2 - street.YCord1);
            if (XSide) return Math.Sin(angle) * ((LineLenght - textblockWidth) / 2);
            else return Math.Cos(angle) * ((LineLenght - textblockWidth) / 2);
        }

    }
}