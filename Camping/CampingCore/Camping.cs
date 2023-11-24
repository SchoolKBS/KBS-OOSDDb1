using System;
using System.Collections.ObjectModel;

namespace CampingCore
{
    public class Camping
    {
        public List<Place> Places { get; set; }
        public List<Reservation> Reservations { get; set; }

        public ICampingRepository CampingRepository { get; private set; }
        public Camping() { }
        public Camping(ICampingRepository campingRepository)
        {
            this.CampingRepository = campingRepository;
            this.Reservations = this.CampingRepository.GetReservations();
            this.Places = this.CampingRepository.GetPlaces();
            this.Reservations = this.CampingRepository.GetReservations();
            this.Places = this.CampingRepository.GetPlaces();


        }
    }
}
