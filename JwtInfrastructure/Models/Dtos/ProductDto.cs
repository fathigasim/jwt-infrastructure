
using JwtInfrastructure.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JwtInfrastructure.Models.Dtos
{
    public class ProductDto
    {
        //[Required]
        [Required(
         ErrorMessageResourceName = "ProductNameRequired"
            ,
       ErrorMessageResourceType = typeof(CommonResources))]
        public string? Name { get; set; }

        [Display(Name = "Name")]
        [Range(1, 20,
        ErrorMessageResourceName = "Less_than_or_exceeded_range_price"
           ,ErrorMessageResourceType = typeof(CommonResources))]
        public decimal Price { get; set; }
    }
}
