namespace TechnicalTestDOT.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public virtual UserModel User { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
