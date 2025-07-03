using System.ComponentModel.DataAnnotations.Schema;

namespace cruddotnet.Models.Entities
{
    public class Spending
    {
        public int SpendingId { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public DateTime SpendingDate { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Value { get; set; }
    }
}
