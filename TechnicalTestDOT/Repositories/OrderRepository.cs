using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TechnicalTestDOT.Contracts;
using TechnicalTestDOT.Data;
using TechnicalTestDOT.Models;
using TechnicalTestDOT.Payloads.Request;
using TechnicalTestDOT.Payloads.Response;

namespace TechnicalTestDOT.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<OrderRepository> _logger;
        private readonly IUserRepository _userRepository;
        public OrderRepository(DatabaseContext context, ILogger<OrderRepository> logger, IUserRepository userRepository) 
        {
            _context = context;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<CommonResponse> CreateOrder(OrderRequest order)
        {
            CommonResponse response = new();
            try
            {
                if (OrderExists(order.InvoiceNumber))
                {
                    response.StatusCode = 400;
                    response.Message = $"duplicate invoice!";
                    _logger.LogWarning(response.Message);
                    return response;
                }

                var user = _userRepository.GetUser(order.Username).Result;
                if (user.StatusCode != 200)
                {
                    return user;
                }
                UserModel userData = (UserModel)user.Data;
                var orderModel = new OrderModel
                {
                    InvoiceNumber = order.InvoiceNumber,
                    ProductName = order.ProductName,
                    Quantity = order.Quantity,
                    UserId = userData.Id,
                    Description = order.Description,
                    CreatedBy = order.CreatedBy,
                    CreatedOn = DateTime.Now
                };
                _context.Orders.Add(orderModel);
                await _context.SaveChangesAsync();
                response.Data = _userRepository.GetUser(userData.Username).Result.Data;
                response.StatusCode = 200;
                response.Message = $"Successfully created an order with invoice number `{order.InvoiceNumber}`";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = $"An error occurred while create an order with invoice number `{order.InvoiceNumber}`";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }

        public async Task<CommonResponse> UpdateOrder(int id, OrderRequest order)
        {
            CommonResponse response = new();
            try
            {

                var user = _userRepository.GetUser(order.Username).Result;
                if (user.StatusCode != 200)
                {
                    return user;
                }
                UserModel userData = (UserModel)user.Data;
                var orderModel = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == id);
                if (orderModel == null)
                {
                    response.StatusCode = 404;
                    response.Message = $"an order with invoice number `{order.InvoiceNumber}` not found";
                    _logger.LogWarning(response.Message);
                    return response;
                }
                orderModel.InvoiceNumber = order.InvoiceNumber;
                orderModel.ProductName = order.ProductName;
                orderModel.Quantity = order.Quantity;
                orderModel.UserId = userData.Id;
                orderModel.Description = order.Description;
                orderModel.UpdatedBy = order.UpdatedBy;
                orderModel.UpdatedOn = DateTime.Now;
                _context.Orders.Attach(orderModel);
                _context.Entry(orderModel).State = EntityState.Modified;
                _context.Entry(orderModel).Property(x => x.CreatedBy).IsModified = false;
                _context.Entry(orderModel).Property(x => x.CreatedOn).IsModified = false;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.InvoiceNumber))
                    {
                        response.StatusCode = 400;
                        response.Message = $"order not found";
                        _logger.LogWarning(response.Message);
                        return response;
                    }
                    else
                    {
                        throw;
                    }
                }
                response.Data = _userRepository.GetUser(userData.Username).Result.Data;
                response.StatusCode = 200;
                response.Message = $"Successfully updated an order with invoice number {order.InvoiceNumber}";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = $"An error occurred while update an order with invoice number `{order.InvoiceNumber}`";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }

        private bool OrderExists(string invoiceNumber)
        {
            return _context.Orders.Any(e => e.InvoiceNumber == invoiceNumber);
        }

        public async Task<CommonResponse> DeleteOrder(string invoiceNumber)
        {
            CommonResponse response = new();
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(u => u.InvoiceNumber == invoiceNumber);
                if (order == null)
                {
                    response.StatusCode = 404;
                    response.Message = $"an order with invoice number `{invoiceNumber}` not found";
                    _logger.LogWarning(response.Message);
                    return response;
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                response.Data = order;
                response.StatusCode = 200;
                response.Message = $"Successfully deleted an order with invoice number `{invoiceNumber}`.";
                _logger.LogInformation(response.Message);
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.StatusCode = 500;
                response.Message = $"An error occurred while delete an order with invoice number `{invoiceNumber}`.";
                _logger.LogError(ex, response.Message);
            }
            return response;
        }
    }
}
