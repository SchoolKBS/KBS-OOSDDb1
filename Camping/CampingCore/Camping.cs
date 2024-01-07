using System;
using System.Collections.ObjectModel;
using CampingCore.CampingRepositories;
using CampingCore.NewFolder;

namespace CampingCore
{
    public class Camping
    {
        private List<Place> Places { get; set;}
        private List<Reservation> Reservations { get; set; }
        private List<Area> Areas { get; set; }
        private List<Street> Streets { get; set; }
        public ICampingRepository CampingRepository { get; set; }
        public Camping(ICampingRepository campingRepository)
        {
            this.CampingRepository = campingRepository;

            this.Reservations = this.CampingRepository.CampingReservationRepository.GetReservations();
            this.Places = this.CampingRepository.CampingPlaceRepository.GetPlaces();
            this.Areas = this.CampingRepository.CampingMapRepository.GetAreas();
            this.Streets = this.CampingRepository.CampingMapRepository.GetStreets();

        }
        
        public void UpdatePlace(int placeID, int streetID, int areaID, bool power, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, bool dogs)
        {
            CampingRepository.CampingPlaceRepository.UpdatePlaceData(placeID, streetID, areaID, power, surfaceArea, pricePerNightPerPerson, amountOfPeople, dogs);
        }
        public List<Area> GetAreas()
        {
            return this.Areas;
        }
        public List<Street> GetStreets()
        {
            return this.Streets;
        }
        public List<Place> GetPlaces()
        {
            return this.Places;
        }
        public List<Reservation> GetReservations()
        {
            return this.Reservations;
        }
    }
}
