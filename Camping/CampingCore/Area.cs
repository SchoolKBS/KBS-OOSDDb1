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
        public int SurfaceArea { get; set; }
        public double PricePerNightPerPerson { get; set; }
        public int AmountOfPeople { get; set; }
        public bool Dogs { get; set; }
        public int Xcord1 { get; set; }
        public int Ycord1 { get; set; }
        public int Xcord2 { get; set; }
        public int Ycord2 { get; set; }

        public Area(ArrayList properties)
        {
            AreaID = (int)properties[0];
            Name = (string)properties[1];
            Power = (bool)properties[2];
            SurfaceArea = (int)properties[3];
            PricePerNightPerPerson = (int)properties[4];
            AmountOfPeople = (int)properties[5];
            Dogs = (bool)properties[6];
            Xcord1 = (int)properties[7];
            Ycord1 = (int)properties[8];
            Xcord2 = (int)properties[9];
            Ycord2 = (int)properties[10];
        }
    }
}
