using System;

namespace CoffeeShopPOS.Models
{
    // Base class for RegularCustomer, SeniorCustomer, or PWDCustomer
    public abstract class Customer
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Regular, Senior, or PWD 
        public DateTime DateRegistered { get; set; }
        public LoyaltyAccount? LoyaltyAccount { get; set; }


        // Every customer type will calculate discounts differently
        // amount parameter = the subtotal before discount
        // returns discount only, not the final price
        public abstract decimal ApplyDiscount(decimal amount);

        public override string ToString()
        {
            return $"{Name} ({Type}) - ID: {Id}";
        }

    }
}