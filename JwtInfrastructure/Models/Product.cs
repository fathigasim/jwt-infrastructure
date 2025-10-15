using System.ComponentModel.DataAnnotations;

namespace JwtInfrastructure.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; }= Guid.NewGuid().ToString();
        [Display(Name="Name"),Required(ErrorMessage ="Name_is_Required")]
        public string Name { get; set; }
        [Display(Name = "Name"), Range(1,20,ErrorMessage ="Less_than_or_exceeded_range_price")]
        public decimal Price { get; set; }
        

        
    }
}
