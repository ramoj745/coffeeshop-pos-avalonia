namespace CoffeeShopPOS.Models
{
    // Base class for all beverage types (BlendedCoffee, IcedCoffee, HotCoffee)
    public abstract class Beverage
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } 
        public string Category { get; set; } = string.Empty;
        
        // each Beverage type will have different base prices
        public abstract decimal CalculatePrice(string size);
    }
}