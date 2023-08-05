namespace WebApplication2.Models
{
    public class Store
    {
        public Guid Id { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string Province { get; set; }
        public HashSet<Laptop> Laptops { get; set; }
    }
}
