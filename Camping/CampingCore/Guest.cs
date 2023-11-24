using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Guest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrepositionName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }

        public Guest(string firstName, string prepositionName, string lastName, string address, string city, string email, string phoneNumber, string postalCode)
        {
            this.FirstName = firstName;
            this.PrepositionName = prepositionName;
            this.LastName = lastName;
            this.Address = address;
            this.City = city;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.PostalCode = postalCode;
        }

    }
}

