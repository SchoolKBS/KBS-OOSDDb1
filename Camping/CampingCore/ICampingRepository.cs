using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public interface ICampingRepository
    {
        public List<Place> GetPlaces();
        public List<Reservation> GetReservations();
        public void RemoveAllReservationsByPlace(Place place);
        public void RemovePlace(Place place);
        public void RemoveReservation(Reservation reservation);
        public void AddReservation(Reservation reservation);
        public void AddGuest(Guest guest);
        public void AddDummyDataPlaces();
        public void AddDummyData();
        public void AddDummyDataReservations(int placeID, int employeeID, int guestID, int i);
        public void AddDummyDataGuests(string firstName, string lastName, string infix, string email, string city, string address, int i);
        public List<Guest> GetGuests();
        public List<Employee> GetEmployees();
        public Place GetPlaceFromPlaceID(int id);
        public Place GetEmployeeFromEmployeeID(int id);
        public Place GetGuestFromGuestID(int id);
        public int GetLastGuestID();
    }
}
