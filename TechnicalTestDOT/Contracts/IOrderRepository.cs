using TechnicalTestDOT.Models;
using TechnicalTestDOT.Payloads.Request;
using TechnicalTestDOT.Payloads.Response;

namespace TechnicalTestDOT.Contracts
{
    public interface IOrderRepository
    {
        Task<CommonResponse> CreateOrder(OrderRequest order);
        Task<CommonResponse> UpdateOrder(int id, OrderRequest order);
        Task<CommonResponse> DeleteOrder(string invoiceNumber);
    }
}
