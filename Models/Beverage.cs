namespace CoffeeShopPOS.Models
{
    public abstract class Beverage
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public string Category { get; set; }
        
        public abstract decimal CalculatePrice(string size);
    }
}