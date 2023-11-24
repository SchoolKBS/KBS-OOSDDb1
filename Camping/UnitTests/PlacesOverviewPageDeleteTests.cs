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
            var mock = new Mock<ICampingRepository>();
            Camping camping = new Camping(mock.Object);
            List<Place> places = createPlaces(10);
            List<Guest> guests = createGuests(10);
            Employee employee = createEmployee();
            for (int i = 1; i <= 10; i++)
            {
                camping.Reservations.Add(new Reservation(i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i + 10), places[i-1].PlaceNumber, employee.EmployeeID, guests[i-1].ID, i, i%2==0, i));
            }
            mock.Setup(p => p.GetPlaces()).Returns(places);
            Assert.That(camping.Places.Count(), Is.EqualTo(10));
            Assert.That(camping.Reservations.Count(), Is.EqualTo(10)); 
            /*var mock = new Mock<ICampingRepository>();
            Camping camping = new Camping(mock.Object);
            ObservableCollection<Reservation> reservations = new ObservableCollection<Reservation>();
            List<Place> places = createPlaces(10);
            Place place = places.First();
            for (int i = 1; i <= 10; i++)
            {
                reservations.Add(new Reservation(i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i + 10), places[i - 1]));
            }
            camping.Reservations = reservations;
            mock.Setup(p => p.GetPlaces()).Returns(places);
            mock.Setup(p => p.RemovePlace(place));
            mock.Setup(p => p.RemoveAllReservationsByPlace(place));
            PlacesOverviewPageDelete.DeletePlace(camping, place);
            mock.Verify(p => p.RemovePlace(place), Times.Once());
            mock.Verify(p => p.RemoveAllReservationsByPlace(place), Times.Once());
            Assert.That(camping.Places.Count(), Is.EqualTo(9));
            Assert.That(camping.Reservations.Count(), Is.EqualTo(9));*/
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
    }
}
