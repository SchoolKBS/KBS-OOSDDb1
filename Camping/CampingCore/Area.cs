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
        public bool Power { get; set; }
        public bool Dogs { get; set; }
        public int SurfaceArea { get; set; }
        public double PricePerNightPerPerson { get; set; }
        public int AmountOfPeople { get; set; }

        // Creates a square for an area
        public int Ycord1 { get; set; } // LEFT TOP
        public int Ycord2 { get; set; } // LEFT BOTTOM
        public int Xcord1 { get; set; } // RIGHT TOP
        public int Xcord2 { get; set; } // RIGHT BOTTOM

        public Area(ArrayList properties)
        {
            AreaID = (int)properties[0];
            Name = (string)properties[1];
            Power = (bool)properties[2];
            SurfaceArea = (int)properties[3];
            PricePerNightPerPerson = (double)properties[4];
            AmountOfPeople = (int)properties[5];
            Dogs = (bool)properties[6];
            Xcord1 = (int)properties[7];
            Ycord1 = (int)properties[8];
            Xcord2 = (int)properties[9];
            Ycord2 = (int)properties[10];
        }

        public int[] GetAreaPositions()
        {
            //  Xcord1 = horizontal
            // Ycord1 = vertical
            // Xcord2 = width
            // Ycord2 = height
            return new int[4] { Xcord1, Ycord1, Xcord2, Ycord2 };
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
