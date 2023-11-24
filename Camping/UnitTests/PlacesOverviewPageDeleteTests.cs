using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
using Microsoft.IdentityModel.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class PlacesOverviewPageDeleteTests
    {
        [Test]
        public void DeletePlace_places_DeletesPlaceAndReservations()
        {

           // List<Place> places = createPlaces(10);
            //List<Reservation> reservations = createReservations();
            var mock = new Mock<ICampingRepository>();
            Camping camping = new Camping(mock.Object);
            //mock.Setup(p => p.GetPlaces()).Returns(places);
            //mock.Setup(p => p.GetReservations()).Returns(reservations);
            Place place = new Place(1, true, 1, 1, 1);

            PlacesOverviewPageDelete.DeletePlace(camping, place);

            mock.Verify(p => p.RemovePlace(place), Times.Once());
            mock.Verify(p => p.RemoveAllReservationsByPlace(place), Times.Once());
        }
        public static List<Place> createPlaces(int amount)
        {
            List<Place> places = new List<Place>();
            for (int i = 0; i < amount; i++)
            {
                Place place = new Place(i, true, i, i, i);
                places.Add(place);
            }
            return places;
        }
        public static Employee createEmployee()
        {
            Employee employee = new Employee(1, "Jan", "Jansen", null);
            return employee;
        }
        public static List<Guest> createGuests(int amount)
        {
            List<Guest> guests = new List<Guest>();
            for (int i = 0; i < amount; i++)
            {
                Guest guest = new Guest(i, "Jan", null, "Jansen", "adres", "stad", "emia@gamil.com", "0612345678", "1221FG");
                guests.Add(guest);
            }
            return guests;
        }
        public static List<Reservation> createReservations()
        {
            List<Reservation> reserverations = new List<Reservation>();
            for (int i = 0; i < 10; i++)
            {
                Reservation reservation = new Reservation(i, DateTime.Now.Date.AddDays(i), DateTime.Now.Date.AddDays(i+1), i, i, i, i, i%2==0, i);
                reserverations.Add(reservation);
            }
            return reserverations;
        }

    }
}
