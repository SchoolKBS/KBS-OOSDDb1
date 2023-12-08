using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Guest
    {
        public int GuestID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Infix { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }

        public Guest(int guestID, string firstName, string prepositionName, string lastName, string address, string city, string email, string phoneNumber, string postalCode)
        {
            this.GuestID = guestID;
            this.FirstName = firstName;
            this.Infix = prepositionName;
            this.LastName = lastName;
            this.Address = address;
            this.City = city;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.PostalCode = postalCode;
        }
        public Guest(string firstName, string prepositionName, string lastName, string address, string city, string email, string phoneNumber, string postalCode) : this(1, firstName, prepositionName, lastName, address, city, email, phoneNumber, postalCode)
        {

        }
        public Guest()
        {

        }
        public Guest(ArrayList properties)
        {
            GuestID = (int)properties[0];
            FirstName = (string)properties[1];
            LastName = (string)properties[2];
            Infix = (string)properties[3];
            Email = (string)properties[4];
            PhoneNumber = (string)properties[5];
            City = (string)properties[6];
            Address = (string)properties[7];
            PostalCode = (string)properties[8];
        }
        public override string ToString()
        {
            string name = FirstName;
            if (!string.IsNullOrEmpty(Infix))
            {
                name += " " + Infix;
            }
            name += " " + LastName;
            return name;
        }
    }
}
