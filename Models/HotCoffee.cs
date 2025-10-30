namespace CoffeeShopPOS.Models
{
    // HotCoffee inherits from Beverage (same case w/ IcedCoffee)
    // This means HotCoffee gets all properties from Beverage
    public class HotCoffee : Beverage
    {
        
        public HotCoffee(string code, string name, decimal basePrice)
        {
            Category = "Hot";  // we set the category automatically
            Code = code;
            Name = name;
            BasePrice = basePrice;
        }
        

        public override decimal CalculatePrice(string size)
        {
            decimal price = BasePrice;
            
            // Hot coffee pricing logic
            switch (size.ToUpper())
            {
                case "S":
                case "SMALL":
                    // Small = base price (no change)
                    break;
                case "M":
                case "MEDIUM":
                    price += 20;
                    break;
                case "L":
                case "LARGE":
                    price += 40;
                    break;
            }
            
            return price;
        }
    }
}