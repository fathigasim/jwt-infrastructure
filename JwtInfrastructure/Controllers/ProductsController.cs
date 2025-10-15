using JwtInfrastructure.Data;
using JwtInfrastructure.Models;
using JwtInfrastructure.Models.Dtos;
using JwtInfrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JwtInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        UserContext dbContext;
        private readonly IStringLocalizer _localizer;
        public ProductsController(UserContext _dbContext, IStringLocalizerFactory factory)
        {
            dbContext = _dbContext;
            _localizer = factory.Create("CommonResources", Assembly.GetExecutingAssembly().GetName().Name);
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<Product>>> Get(string? q = "", string? sort = "", int page = 1, int pageSize = 5)
        {
            var query = dbContext.Products.AsQueryable();
            if (!string.IsNullOrEmpty(q))
                query = query.Where(p => p.Name.Contains(q));

            if (sort == "lowToHigh")
                query = query.OrderBy(p => p.Price);
            else if (sort == "highToLow")
                query = query.OrderByDescending(p => p.Price);

            var totalItems = query.Count();
           // var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = new PagedResult<Product>
            {
                Items = model,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = dbContext.Products.Count()
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody][Bind("Name,Price")] ProductDto productdto)
        {
            try
            {
                // Check model validation errors first
               // if (!ModelState.IsValid)
               // {
                    //var errors = ModelState
                    //    .Where(kvp => kvp.Value.Errors.Any())
                    //    .ToDictionary(
                    //        kvp => kvp.Key,
                    //        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    //    );

                    //return BadRequest(new { errors });
             //   }

                // Ensure productdto is not null
                if (productdto == null)
                {
                    return BadRequest(new { message = _localizer["ProductNull"].Value });
                }

                var product = new Product
                {
                    Name = productdto.Name,
                    Price = productdto.Price
                };

                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();

                var successMsg = _localizer["ProductAdd"].Value;
                return Ok(new { product, message = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = _localizer["UnexpectedError"].Value });
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id ,ProductDto productdto)
        {
            //Console.WriteLine($"Raw JSON: {await new StreamReader(Request.Body).ReadToEndAsync()}");
            if (id == null)
                return BadRequest("Product not found");

            var product = new Product
            {
                Name = productdto.Name,
                Price = productdto.Price
            };
            var producttoUpdate=await dbContext.Products.Where(p=>p.Id == id).FirstOrDefaultAsync();
            if (producttoUpdate != null)
            {
                producttoUpdate.Price = productdto.Price;
                producttoUpdate.Name = productdto.Name;
            }
             dbContext.Products.Update(producttoUpdate);
            await dbContext.SaveChangesAsync();

            return Ok(producttoUpdate); // ✅ return the updated product
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Product not found");
            var model = await dbContext.Products.Where(p => p.Id.Equals(id)).FirstOrDefaultAsync();

             dbContext.Products.Remove(model);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
