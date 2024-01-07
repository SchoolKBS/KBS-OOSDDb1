using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public enum AreaColor
    {
        Red,
        Orange, 
        Yellow,
        Green,
        Blue,
        Purple,
        Pink,
        Violet,
    }
    public class Area : Facilities
    {
        public int AreaID { get; set; }
        public string Name { get; set; }
        public int Color {  get; set; }
        public int YCord1 { get; set; } 
        public int XCord1 { get; set; }
        public int Width { get; set; } 
        public int Height { get; set; } 
        public Area()
        {
            Color = 9;
        }
        public Area(ArrayList properties)
        {
            AreaID = (int)properties[0];
            Name = (string)properties[1];
            Color = (int)properties[2];
            Power = (bool)properties[3];
            Dogs = (bool)properties[4];
            SurfaceArea = (int)properties[5];
            PricePerNightPerPerson = (double)properties[6];
            AmountOfPeople = (int)properties[7];    
            XCord1 = (int)properties[8];
            YCord1 = (int)properties[9];
            Width = (int)properties[10];
            Height = (int)properties[11];
        }
        public Area(int areaID, string name, int color, bool power, bool dogs, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, int xCord1, int yCord1, int width, int height)
        {
            AreaID = areaID;
            Name = name;
            Color = color;
            Power = power;
            Dogs = dogs;
            SurfaceArea = surfaceArea;
            PricePerNightPerPerson = pricePerNightPerPerson;
            AmountOfPeople = amountOfPeople;
            XCord1 = xCord1;
            YCord1 = yCord1;
            Width = width;
            Height = height;
        }

        public int[] GetAreaPositions()
        {
            return new int[4] { XCord1, YCord1, Width, Height };
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
