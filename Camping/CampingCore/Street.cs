using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Street
    {
        public int StreetNumber { get; set; }
        public string Name { get; set; }
        public bool HasPower { get; set; }
        public bool CanHaveDogs { get; set; }
        public int Xcord1 { get; set; }
        public int Ycord1 { get; set; }
        public int Xcord2 { get; set; }
        public int Ycord2 { get; set; }
        public List<Place> Places { get; set; }
    }
}
