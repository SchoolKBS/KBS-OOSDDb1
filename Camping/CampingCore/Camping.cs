using System;
using System.Collections.ObjectModel;

namespace CampingCore
{
    public class Camping
    {
        public ObservableCollection<Place> Places { get; set; }
        public ObservableCollection<Reservation> Reservations { get; set; }

        public Camping()
        {
            this.Places = new ObservableCollection<Place>();
            this.Reservations = new ObservableCollection<Reservation>();
            Random random = new Random();

            for (int i = 1; i <= 10; i++)
            {
                Places.Add(new Place(i, true, 1, 1, 1));

                // Generate random start date
                DateTime startDate = DateTime.Now.AddDays(random.Next(1, 11));

                // Generate random end date based on start date
                DateTime endDate = startDate.AddDays(random.Next(1, 11));

                Reservations.Add(new Reservation(i, startDate, endDate, Places[i - 1]));
            }
        }
    }
}
