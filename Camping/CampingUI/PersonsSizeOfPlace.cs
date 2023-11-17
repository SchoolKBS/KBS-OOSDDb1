using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingUI
{
    public class PersonsSizeOfPlace : ObservableCollection<string>
    {
        public PersonsSizeOfPlace()
        {
            Add("2 personen");
            Add("4 personen");
            Add("6 personen");
            Add("8 of meer personen");
        }
    }
}
