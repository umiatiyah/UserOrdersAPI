using TechnicalTestDOT.Models;

namespace TechnicalTestDOT.Payloads.Request
{
    public class OrderRequest
    {
        public string InvoiceNumber { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
