namespace CampingCore
{
    public class Employee : Person
    {
        public int EmployeeID { get; set; }

        public Employee(int employeeID, string firstName, string lastName, string infix) : base(firstName, lastName, infix)
        {
            this.EmployeeID = employeeID;
        }
    }
}
