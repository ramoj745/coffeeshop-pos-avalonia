namespace CoffeeShopPOS.Models
{
    // inherit from Customer
    public class RegularCustomer : Customer
    {
        public RegularCustomer(string id, string name)
        {
            Type = "Regular";
            Id = id;
            Name = name;
        }


        // no discount for regular customers
        public override decimal ApplyDiscount(decimal amount)
        {
            return 0;
        }
    }
}