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
            Place place = new Place(12, true, 10, true, 10, 2, 25, 1, 1);
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            camping.Places.Add(place);

            Assert.IsTrue(camping.Places.Contains(place));
            Assert.IsTrue(place.Power);
        }

        [Test]
        public void AddInvalidPlace()
        {
            Assert.Throws<FormatException>(() =>
            {
                Place place = new Place(12, true, int.Parse("jaapstraat"), true, 10, 10, 10, 10, 10);
        });
        }
    }
}
