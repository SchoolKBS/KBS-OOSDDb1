using CampingCore.CampingRepositories;
using CampingCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class TestSupportClass
    {
        public static List<Place> CreatePlaces()
        {
            List<Place> listToCheck = new List<Place>();
            for (int i = 1; i <= 5; i++)
            {
                listToCheck.Add(new Place(i, true, i, i, true, i, i, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Place(i, false, i, i, false, i, i, i, i, i));
            }
            return listToCheck;
        }
        public static List<Reservation> CreateReservations()
        {
            List<Reservation> listToCheck = new List<Reservation>();
            for (int i = 1; i <= 5; i++)
            {
                listToCheck.Add(new Reservation(i, DateTime.Now.Date.AddDays(i), DateTime.Now.Date.AddDays(i + 1), i, i, i, true, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Reservation(i, DateTime.Now.Date.AddDays(i), DateTime.Now.Date.AddDays(i + 2), i, i, i, false, i));
            }
            return listToCheck;
        }
        public static List<Area> CreateAreas()
        {
            List<Area> listToCheck = new List<Area>();
            for (int i = 0; i < 5; i++)
            {
                listToCheck.Add(new Area(i, "Zwolle" + i, i, i % 2 == 0, i % 2 == 0, i, i, i, i, i, i, i));
            }
            return listToCheck;
        }
        public static Mock<ICampingRepository> MockIcampingRepository()
        {
            var campingRepositoryMock = new Mock<ICampingRepository>();
            campingRepositoryMock.Setup(m => m.CampingPlaceRepository.GetPlaces()).Returns(CreatePlaces());
            campingRepositoryMock.Setup(m => m.CampingReservationRepository.GetReservations()).Returns(CreateReservations());
            campingRepositoryMock.Setup(m => m.CampingMapRepository.GetAreas()).Returns(CreateAreas());
            return campingRepositoryMock;
        }
    }
}
