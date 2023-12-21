using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.CampingRepositories
{
    public interface ICampingReservationRepository
    {
        public List<Reservation> GetReservations();
        public void RemoveAllPreviousReservationsByPlace(Place place, DateTime departureDate);
        public void RemoveReservation(Reservation reservation);
        public void AddReservation(Reservation reservation);
        public Guest GetGuestFromGuestID(int id);
        public void UpdateReservation(Reservation reservation);

    }
}
