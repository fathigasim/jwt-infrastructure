using JwtInfrastructure.Data;
using JwtInfrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace JwtInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        IConfiguration configuration;
        UserContext dbContext;
        public PaymentController(IConfiguration _configuration, UserContext _dbContext)
        {
            configuration = _configuration;
            dbContext = _dbContext;
        }
        [Authorize]
        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession([FromBody] List<CartItemDto> items)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:StripeKey"]; // secret key
            var domain = "http://localhost:5173";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = items.Select(i => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(i.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = i.Name,
                        }
                    },
                    Quantity = i.Quantity,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = domain + "/success",
                CancelUrl = domain + "/cancel"
            };
            try
            {
                using (var transaction=dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var order = new Order();
                        dbContext.Order.Add(order);
                        dbContext.SaveChanges();
                         

                        var orderItems = (from i in items
                                          select
                         new OrderItems() { Name = i.Name, OrderId = order.Id, Price = i.Price, Quantity = i.Quantity }).ToList();
                        dbContext.OrderItems.AddRange(orderItems);
                        dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex) { 
                       transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex) { 
                
            }

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { id = session.Id, url = session.Url });
        }
    }

    public class CartItemDto
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}
