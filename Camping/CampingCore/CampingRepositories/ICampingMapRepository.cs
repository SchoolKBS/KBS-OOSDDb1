﻿using System;
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
        public Street GetStreetByStreetName(string streetName);
        public Area GetAreaByAreaName(string areaName);

        public Area GetAreaByAreaID(Place place);
        public void AddExtend(int placeID, bool? power, bool? dogs, bool? surfaceArea, bool? pricePerNightPerPerson, bool? amountOfPeople);
    }
}
