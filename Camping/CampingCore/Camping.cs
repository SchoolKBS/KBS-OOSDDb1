using System.Collections.ObjectModel;

namespace CampingCore
{
    public class Camping
    {
        public List<Place> Places {
            get; //{ get uit database} 
            set; }
        public ObservableCollection<Reservation> Reservations { get; set; }

        public Camping()
        {
            this.Places = new List<Place>();
            //this.Places = GetPlaces();
            this.Reservations = new ObservableCollection<Reservation>();
            for (int i = 1; i <= 10; i++)
            {
                Places.Add(new Place(i, true, 1, i * 2, i));
                Reservations.Add(new Reservation(i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i + 10), Places[i - 1]));
            }
        }
    }
}
