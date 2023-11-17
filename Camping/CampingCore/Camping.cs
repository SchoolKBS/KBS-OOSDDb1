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

        public Camping()
        {
            this.Places = new ObservableCollection<Place>();
            for(int i = 1; i <= 10; i++)
            {
                Places.Add(new Place(i, true, 1, 1, 1));
            }
        }
    }
}
