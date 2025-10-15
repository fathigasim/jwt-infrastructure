using System.ComponentModel.DataAnnotations;

namespace JwtInfrastructure.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }= DateTime.Now;
        public  ICollection< OrderItems> OrderItems{ get; set; }=new List< OrderItems>();

    }


}
