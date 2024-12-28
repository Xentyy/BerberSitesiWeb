namespace BerberSite.Models
{
    public class EmployeeOperation
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int OperationId { get; set; }
        public Operation Operation { get; set; }
    }
}
