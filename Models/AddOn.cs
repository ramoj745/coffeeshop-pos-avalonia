namespace CoffeeShopPOS.Models
{
    public class AddOn
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public AddOn(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        
        // For debugging 
        public override string ToString()
        {
            return $"{Name} - ₱{Price:F2}";
        }
    }
}