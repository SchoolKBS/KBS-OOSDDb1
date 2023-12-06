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
        public void RemoveAllPreviousReservationsByPlace(Place place, DateTime departureDate);
        public void RemovePlace(Place place);
        public void RemoveReservation(Reservation reservation);
        public void AddReservation(Reservation reservation);
        public void AddGuest(Guest guest);
        public void AddDummyDataArea();
        public void AddDummyDataStreet();
        public void AddDummyDataPlaces();
        public void AddDummyData();
        public void AddDummyDataReservations(int placeID, int i);
        public void AddDummyDataGuests(string firstName, string lastName, string infix, string email, string city, string address, int i);
        public List<Guest> GetGuests();
        public List<Street> GetStreets();
        public List<Area> GetAreas();
        public Place GetPlaceFromPlaceID(int id);
        public Place GetGuestFromGuestID(int id);
        public int GetLastGuestID();
        public void AddPlace(Place place);
        public void UpdatePlaceData(Place place, bool power, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, bool dogs);
        public Street GetStreetByStreetID(Place place);
    }
}
