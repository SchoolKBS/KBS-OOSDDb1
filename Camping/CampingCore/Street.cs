using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Street
    {
        public int StreetID { get; set; }
        public int AreaID { get; set; }
        public string Name { get; set; }
        public bool Power { get; set; }
        public bool Dogs { get; set; }
        public int Xcord1 { get; set; }
        public int Ycord1 { get; set; }
        public int Xcord2 { get; set; }
        public int Ycord2 { get; set; }

        public Street(ArrayList properties)
        {
            StreetID = (int)properties[0];
            AreaID = (int)properties[1];
            Name = (string)properties[2];
            Power = (bool)properties[3];
            Dogs = (bool)properties[4];
            Xcord1 = (int)properties[5];
            Ycord1 = (int)properties[6];
            Xcord2 = (int)properties[7];
            Ycord2 = (int)properties[8];

        }

        public int[] GetStreetPositions()
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