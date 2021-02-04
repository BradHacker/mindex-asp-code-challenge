using System;
namespace challenge.Models
{
    public class Compensation
    {
        public String CompensationId { get; set; }
        public Employee Employee { get; set; }
        public int Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
