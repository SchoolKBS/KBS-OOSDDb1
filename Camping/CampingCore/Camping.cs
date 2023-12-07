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
    }
}
