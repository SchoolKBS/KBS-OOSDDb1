using System.Collections.ObjectModel;

namespace CampingCore
{
    public class Camping
    {
        public List<Place> Places { get; set;}
        public ObservableCollection<Reservation> Reservations { get; set; }
        public ICampingRepository CampingRepository { get; private set; }

        public Camping(ICampingRepository campingRepository)
        {
            this.CampingRepository = campingRepository;
            this.Reservations = new ObservableCollection<Reservation>();
            this.Places = this.CampingRepository.GetPlaces();
            for (int i = 1; i <= 10; i++)
            {
                //Places.Add(new Place(i, true, 1, i * 2, i));
                Reservations.Add(new Reservation(i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i + 10), Places[i - 1]));
            }
            this.Places = this.CampingRepository.GetPlaces();

        }
    }
}
