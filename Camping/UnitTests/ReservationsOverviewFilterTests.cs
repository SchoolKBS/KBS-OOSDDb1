using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampingCore;
using CampingCore.CampingRepositories;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using CampingUI;

namespace UnitTests
{
    public class ReservationOverviewFilter
    {
        [Test]
        public void GetFilteredListOn_IsPaid_ReturnsList()
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Reservation> reservations = new List<Reservation>();
            
            //Act
            reservations = camping.CampingRepository.CampingReservationRepository.GetReservations().Where(reservation => reservation.IsPaid == true);
            
            //Assert
            Assert.That(reservations.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GetFilteredListOn_IsNotPaid_ReturnsList()
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Reservation> reservations = new List<Reservation>();

            //Act
            reservations = camping.CampingRepository.CampingReservationRepository.GetReservations().Where(reservation => reservation.IsPaid == false);
            
            //Assert
            Assert.That(reservations.Count(), Is.EqualTo(5));
        }
        [Test]

        public void GetFilteredListOn_ReservationNumber_ReturnsList()
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Reservation> reservations = new List<Reservation>();
            
            //Act
            reservations = camping.CampingRepository.CampingReservationRepository.GetReservations().Where(reservation => reservation.ReservationID == 1);
            
            //Assert
            Assert.That(reservations.Count(), Is.EqualTo(1));
        }

        [Test]

        public void GetFilteredListOn_PlaceNumber_ReturnsList()
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Reservation> reservations = new List<Reservation>();
            
            //Act
            reservations = camping.CampingRepository.CampingReservationRepository.GetReservations().Where(reservation => reservation.PlaceID == 1);
            
            //Assert
            Assert.That(reservations.Count(), Is.EqualTo(1));
        }
        [Test]

        public void GetFilteredListOn_ArrivalDate_ReturnsList()
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Reservation> reservations = new List<Reservation>();
            
            //Act
            reservations = camping.CampingRepository.CampingReservationRepository.GetReservations().Where(reservation => reservation.ArrivalDate >= DateTime.Now.AddDays(2));
            
            //Assert
            Assert.That(reservations.Count(), Is.EqualTo(8));
        }
        [Test]

        public void GetFilteredListOn_LeaveDate_ReturnsList()
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Reservation> reservations = new List<Reservation>();
            
            //Act
            reservations = camping.CampingRepository.CampingReservationRepository.GetReservations().Where(reservation => reservation.DepartureDate <= DateTime.Now.AddDays(3));
            
            //Assert
            Assert.That(reservations.Count(), Is.EqualTo(2));
        }
        [Test]

        public void GetFilteredListOn_GuestNameFull_ReturnsList()
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Reservation> reservations = new List<Reservation>();
            
            //Act
            reservations = camping.CampingRepository.CampingReservationRepository.GetReservations().Where
    (reservation =>
          reservation.GuestID.Equals(6)
      ); 

            //Assert
            Assert.That(reservations.Count(), Is.EqualTo(1));
        }
    }
}

