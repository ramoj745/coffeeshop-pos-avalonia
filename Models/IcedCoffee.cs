namespace CoffeeShopPOS.Models
{

    public class IcedCoffee : Beverage
    {
        public IcedCoffee()
        {
            Category = "Iced";  
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