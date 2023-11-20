using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            for(int i = 1; i <= 10; i++)
            {
                Places.Add(new Place(i, true, 1, i*2, i));
                Reservations.Add(new Reservation(i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i + 10), Places[i - 1]));
            }
        }
    }
}
