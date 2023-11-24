using System;
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
            for (int i = 1; i <= 5; i++)
            {
                Reservations.Add(new Reservation(i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i + 10), Places[i].PlaceNumber, i, i, i, i%2==0, i));
            }
            this.Places = this.CampingRepository.GetPlaces();

                // Generate random start date
                DateTime startDate = DateTime.Now.AddDays(random.Next(1, 11));

                // Generate random end date based on start date
                DateTime endDate = startDate.AddDays(random.Next(1, 11));

                Reservations.Add(new Reservation(i, startDate, endDate, Places[i - 1]));
            }
        }
    }
}
