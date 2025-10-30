namespace CoffeeShopPOS.Models
{
    // inherit from Customer
    public class PWDCustomer : Customer
    {
        public PWDCustomer(string id, string name)
        {
            Type = "PWD";
            Id = id;
            Name = name;
        }

        public override decimal ApplyDiscount(decimal amount)
        {
            return amount * 0.20m;
        }
    }
}