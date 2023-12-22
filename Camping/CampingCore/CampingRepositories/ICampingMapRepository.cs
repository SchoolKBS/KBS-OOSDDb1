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
        public Street GetStreetByStreetID(int streetID);
        public Street GetStreetByStreetName(string streetName);
        public Area GetAreaByAreaName(string areaName);
        public Area GetAreaByAreaID(Place place);
        public void AddExtend(int placeID, bool? power, bool? dogs, bool? surfaceArea, bool? pricePerNightPerPerson, bool? amountOfPeople);
        public void AddNewStreet(string name, bool power, bool dogs, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, int xCord1, int yCord1, int xCord2, int yCord2);
        public void UpdateStreetByStreetID(string streetName, bool power, bool dogs, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, int streetID);
        public void AddNewArea(Area area);
    }
}
