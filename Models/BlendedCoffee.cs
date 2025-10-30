namespace CoffeeShopPOS.Models
{

    public class BlendedCoffee : Beverage
    {
        public BlendedCoffee(string code, string name, decimal basePrice)
        {
            Category = "Blended";
            Code = code;
            Name = name;
            BasePrice = basePrice;
        }
        

        public override decimal CalculatePrice(string size)
        {
            decimal price = BasePrice;
            
            switch (size.ToUpper())
            {
                case "S":
                case "SMALL":
                    break;
                case "M":
                case "MEDIUM":
                    price += 30;
                    break;
                case "L":
                case "LARGE":
                    price += 50;
                    break;
            }
            
            return price;
        }
    }
}