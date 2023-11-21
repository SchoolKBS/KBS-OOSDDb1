using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Guest : Person
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public Guest(string firstName, string prepositionName, string lastName, string address, string city, string email, string phoneNumber) 
            : base(firstName, prepositionName, lastName)
        {
            
        }

    }
}
