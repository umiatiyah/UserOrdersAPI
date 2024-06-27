using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TechnicalTestDOT.Contracts;
using TechnicalTestDOT.Models;
using TechnicalTestDOT.Payloads.Request;
using TechnicalTestDOT.Payloads.Response;

namespace TechnicalTestDOT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository) 
        {
            _orderRepository = orderRepository;
        }
        
        [HttpPost]
        public async Task<ActionResult<CommonResponse>> CreateOrder(OrderRequest order)
        {
            var data = await _orderRepository.CreateOrder(order);
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else
            {
                return BadRequest(data);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CommonResponse>> UpdateOrder(int id, OrderRequest order)
        {
            var data = await _orderRepository.UpdateOrder(id, order);
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else if (data.StatusCode == 404)
            {
                return NotFound(data);
            }
            else
            {
                return BadRequest(data);
            }
        }
        [HttpDelete("{invoiceNumber}")]
        public async Task<ActionResult<CommonResponse>> DeleteOrder(string invoiceNumber)
        {
            var data = await _orderRepository.DeleteOrder(invoiceNumber);
            if (data.StatusCode == 200)
            {
                return Ok(data);
            }
            else if (data.StatusCode == 404)
            {
                return NotFound(data);
            }
            else
            {
                return BadRequest(data);
            }
        }
    }
}
