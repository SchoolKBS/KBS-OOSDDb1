using System;
using System.Collections.ObjectModel;
using CampingCore.CampingRepositories;
using CampingCore.NewFolder;

namespace CampingCore
{
    public class Camping
    {
        public List<Place> Places { get; set;}
        public List<Reservation> Reservations { get; set; }
        public ICampingRepository CampingRepository { get; set; }
        public Camping(ICampingRepository campingRepository)
        {
            this.CampingRepository = campingRepository;

            this.Reservations = this.CampingRepository.CampingReservationRepository.GetReservations();
            this.Places = this.CampingRepository.CampingPlaceRepository.GetPlaces();

        }
        
        public void UpdatePlace(int placeID, int streetID, bool power, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, bool dogs)
        {
            CampingRepository.CampingPlaceRepository.UpdatePlaceData(placeID, streetID, power, surfaceArea, pricePerNightPerPerson, amountOfPeople, dogs);
        }
    }
}
