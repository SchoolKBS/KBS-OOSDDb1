using CampingCore;
using Moq;

namespace UnitTests
{
    public class PlacesOverviewAddPlaceTests
    {
        [Test]
        public void AddValidPlace()
        {
            int PlaceNumber = 12;
            int SurfaceArea = 10;
            int PricePerPersonPerNight = 25;
            int AmountOfPeople = 2;
            string Description = "Een plaats met mooi uiticht";
            string Electricity = "ja";
            bool HasElectricity;

            if (Electricity.Equals("ja"))
            {
                HasElectricity = true;
            }
            else
            {
                HasElectricity = false;
            }

            Place place = new Place(PlaceNumber, HasElectricity, SurfaceArea, AmountOfPeople, PricePerPersonPerNight, Description);
            Camping camping = new Camping();
            camping.Places.Add(place);

            Assert.IsTrue(camping.Places.Contains(place));
            Assert.IsTrue(place.Power);
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
