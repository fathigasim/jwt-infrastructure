using JwtInfrastructure.Data;
using JwtInfrastructure.Models;
using JwtInfrastructure.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        UserContext dbContext;
        public OrderController(UserContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var orders = await dbContext.Order.Include(p => p.OrderItems).ToListAsync();
           return Ok(orders);
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetAsync(DateTime date)
        {
            var orders = await dbContext.Order.Include(p => p.OrderItems).Where(p=>p.OrderDate.Date==date.Date).ToListAsync();
            return Ok(orders);
        }

        //[HttpPost]
        //public async Task<IActionResult> Post()
        //{
        //    var order = new Order();
        //   await dbContext.Order.AddAsync(order);
        //    await dbContext.SaveChangesAsync();
        //    return Ok();

        //}
    }
}
