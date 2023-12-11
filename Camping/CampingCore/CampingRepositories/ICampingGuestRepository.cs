using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.NewFolder
{
    public interface ICampingGuestRepository
    {
        public void AddGuest(Guest guest);
        public List<Guest> GetGuests();
        public int GetLastGuestID();
        public Place GetGuestFromGuestID(int id);

        public List<Guest> GetGuestsByFirstAndLastName(string FirstName, string LastName);

        public List<Guest> GetGuestsByFirstName(string FirstName);

        public List<Guest> GetGuestsByLastName(string LastName);

        public void UpdateGuest(Guest guest);
    }
}
