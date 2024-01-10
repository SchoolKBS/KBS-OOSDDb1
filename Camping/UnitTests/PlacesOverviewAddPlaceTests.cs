using CampingCore;
using CampingCore.CampingRepositories;
using CampingCore.PlacesOverviewPageClasses;
using Moq;

namespace UnitTests
{
    public class PlacesOverviewAddPlaceTests
    {
        [Test]
        public void AddValidPlace()
        {
            //Arrange
            Place place = new Place(12, true, 10, 10, true, 10, 2, 25, 1, 1);
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);

            //Act
            camping.GetPlaces().Add(place);

            //Assert
            Assert.IsTrue(camping.GetPlaces().Contains(place));
            Assert.IsTrue(place.Power);
        }

        [Test]
        public void AddInvalidPlace()
        {
            //Act, Assert
            Assert.Throws<FormatException>(() =>
            {
                Place place = new Place(12, true, int.Parse("jaapstraat"), 10, true, 10, 10, 10, 10, 10);
        });
        }
    }
}
