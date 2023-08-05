using System.Reflection.Metadata.Ecma335;

namespace WebApplication2.Models
{
    public class Laptop
    {
        public Guid Id { get; set; }

        private string _modelName;
        
        public string ModelName
        {
            get => _modelName;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Laptop model name must be at least three characters in length.");
                }
                else
                {
                    _modelName = value;
                }
            }
        }

        private decimal _price;
        public int QuantityInStock { get; set; }

        public decimal Price { get => _price; 
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Price cannot be less than zero.");
                }

                _price = value;
            }
        }
        
        public LaptopCondition Condition { get; set; }
        
        public Guid BrandId { get; set; }
        
        public Brand Brand { get; set; }

        public ICollection<StoreLaptop> StoreLaptops { get; set; } = new HashSet<StoreLaptop>();
    }

    public enum LaptopCondition
    {
        New,
        Refurbished,
        Rental
    }
}
