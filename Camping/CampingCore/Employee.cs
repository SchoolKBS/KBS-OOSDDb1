namespace CampingCore
{
    public class Employee : Person
    {
        public int EmployeeID { get; set; }

        public Employee(string firstName, string prepositionName, string lastName, int employeeID) : base(firstName, prepositionName, lastName)
        {
            this.EmployeeID = employeeID;
        }
    }
}
