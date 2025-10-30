namespace CoffeeShopPOS.Models
{
    // inherit from Customer
    public class SeniorCustomer : Customer
    {
        public SeniorCustomer(string id, string name)
        {
            Type = "Senior";
            Id = id;
            Name = name;
        }

        public override decimal ApplyDiscount(decimal amount)
        {
            return amount * 0.20m;
        }
    }
}