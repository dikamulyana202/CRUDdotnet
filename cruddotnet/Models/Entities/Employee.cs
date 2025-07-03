using System.ComponentModel.DataAnnotations;

namespace cruddotnet.Models.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        
        [MaxLength(100)]
        public string EmployeeName { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public ICollection<Spending> Spendings { get; set; }
    }
}
