using cruddotnet.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace cruddotnet.Models
{
    public class AddDepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
