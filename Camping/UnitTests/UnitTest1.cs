using CampingCore;
namespace UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Camping camping = new Camping();
            for (int i = 0; i < 10; i++)
            {
                camping.Places.Add(new Place(1, true, 1, 1, 1));
            }

            Assert.That(camping.Places.Count(), Is.EqualTo(10));
        }
    }
}