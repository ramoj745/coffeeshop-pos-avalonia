namespace CoffeeShopPOS.Models
{

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