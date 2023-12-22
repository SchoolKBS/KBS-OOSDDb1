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
        public void UpdatePlaceData(int placeID, int streetID, int areaID, bool power, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, bool dogs);
        public List<bool?> GetPlaceExtendingByPlaceID(int placeID);
        public void RemovePlaceExtends(Place place);
        public void UpdatePlaceDataExtending(int placeID, bool? power, bool? dogs, bool? surfaceArea, bool? pricePerNightPerPerson, bool? amountOfPeople);
    }
}
