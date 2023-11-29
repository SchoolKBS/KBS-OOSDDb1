using CampingCore;
using Moq;

namespace UnitTests
{
    public class PlacesOverviewAddPlaceTests
    {
        [Test]
        public void AddValidPlace()
        {
            int placeNumber = 12;
            int surfaceArea = 10;
            int pricePerPersonPerNight = 25;
            int amountOfPeople = 2;
            bool hasPower = true;
            Place place = new Place(placeNumber, hasPower, surfaceArea, amountOfPeople, pricePerPersonPerNight);
            Assert.IsTrue(place.HasPower);
        }

        [Test]
        public void AddInvalidPlace()
        {
            int PlaceNumber = 12;
            string SurfaceArea = "tien";
            int PricePerPersonPerNight = 25;
            int AmountOfPeople = 2;
            string Description = "Een plaats met mooi uiticht";
            bool HasElectricity = true;


            Assert.Throws<FormatException>(() =>
            {
                Place place = new Place(PlaceNumber, HasElectricity, int.Parse(SurfaceArea), AmountOfPeople, PricePerPersonPerNight, Description);
            });
        }
    }
}
