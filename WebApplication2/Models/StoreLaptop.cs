namespace WebApplication2.Models
{
    public class StoreLaptop
    {
        public Guid StoreId { get; set; }
        public Store Store { get; set; }
        public Guid LaptopId { get; set; }
        public Laptop Laptop { get; set; }
    }
}
