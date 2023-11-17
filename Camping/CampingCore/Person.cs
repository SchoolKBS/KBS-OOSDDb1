using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public abstract class Person
    {
        protected string FirstName { get; set; }
        protected string PrepositionName { get; set; } //E.g. van, van der, etc.
        protected string LastName {  get; set; }

        public Person(string firstName, string prepositionName, string lastName)
        {
            this.FirstName = firstName;
            this.PrepositionName = prepositionName;
            this.LastName = lastName;
        }
    }
}
