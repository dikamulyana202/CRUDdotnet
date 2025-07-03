using System.ComponentModel.DataAnnotations;

namespace cruddotnet.Models.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
