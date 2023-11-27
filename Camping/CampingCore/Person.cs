namespace CampingCore
{
    public abstract class Person
    {
        protected string FirstName { get; set; }
        protected string Infix { get; set; } //E.g. van, van der, etc.
        protected string LastName { get; set; }

        public Person(string firstName, string lastName, string infix)
        {
            this.FirstName = firstName;
            this.Infix = infix;
            this.LastName = lastName;
        }
    }
}
