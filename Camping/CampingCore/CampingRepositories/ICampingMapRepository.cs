using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.CampingRepositories
{
    public interface ICampingMapRepository
    {
        public List<Street> GetStreets();
        public List<Area> GetAreas();
        public Street GetStreetByStreetID(Place place);
        public Street GetSteetByStreetName(string streetName);
    }
}
