using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.CampingRepositories
{
    public interface ICampingPlaceRepository
    {
        public List<Place> GetPlaces();
        public void RemovePlace(Place place);
        public void AddPlace(Place place);
        public Place GetPlaceFromPlaceID(int id);
        public void UpdatePlaceData(Place place, bool power, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, bool dogs);

    }
}
