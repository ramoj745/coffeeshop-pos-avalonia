namespace CoffeeShopPOS.Models
{
    // inherit from Beverage
    public class IcedCoffee : Beverage
    {
        public IcedCoffee(string code, string name, decimal basePrice)
        {
            Category = "Iced";
            Code = code;
            Name = name;
            BasePrice = basePrice;
        }
        

        public override decimal CalculatePrice(string size)
        {
            // Iced coffee pricing logic
            decimal price = BasePrice;
            
            switch (size.ToUpper())
            {
                case "S":
                case "SMALL":
                    break;
                case "M":
                case "MEDIUM":
                    price += 25;
                    break;
                case "L":
                case "LARGE":
                    price += 45;
                    break;
            }
            
            return price;
        }
    }
}