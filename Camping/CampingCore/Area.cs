using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Area
    {
        public int AreaID { get; set; }
        public string Name { get; set; }
        public int Color {  get; set; }
        public bool Power { get; set; }
        public bool Dogs { get; set; }
        public int SurfaceArea { get; set; }
        public double PricePerNightPerPerson { get; set; }
        public int AmountOfPeople { get; set; }
        public int YCord1 { get; set; } 
        public int XCord1 { get; set; }
        public int Width { get; set; } 
        public int Height { get; set; } 

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
