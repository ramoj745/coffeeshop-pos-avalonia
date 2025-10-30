using System.Collections.Generic;
using System.Linq;

namespace CoffeeShopPOS.Models
{
    // OrderItem represents one item in a customer's order
    // combines Beverage + Size + Quantity + AddOns
    public class OrderItem
    {
        // can be HotCoffee, IcedCoffee, or BlendedCoffee
        public Beverage Beverage { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }

        // list of addons (extra shot, whipped cream, etc.)
        public List<AddOn> AddOns { get; set; }

        // initiates essential data to create an order item
        public OrderItem(Beverage beverage, string size, int quantity)
        {
            Beverage = beverage;
            Size = size;
            Quantity = quantity;

            // customer can add to this later
            AddOns = new List<AddOn>();
        }

        // add to AddOn list
        public void AddAddOn(AddOn addOn)
        {
            AddOns.Add(addOn);
        }

        // calculate total price of order
        public decimal GetTotalPrice()
        {
            // get base price for the beverage with selected size
            // each beverage type calculates differently
            decimal beveragePrice = Beverage.CalculatePrice(Size);

            // adds up all add-on prices
            decimal addOnsTotal = AddOns.Sum(addon => addon.Price);

            // multiply by quantity
            decimal totalPrice = (beveragePrice + addOnsTotal) * Quantity;

            return totalPrice;
        }

        // get readable description of order item
        public string GetDescription()
        {
            string description = $"{Beverage.Name} ({Size})";

            if (AddOns.Any())
            {
                // adds add-ons in a format like (Whipped Cream, Extra Shot, ...)
                string addOnsList = string.Join(", ", AddOns.Select(a => a.Name));
                description += $"\n + {addOnsList}";
            }

            description += $"\nQuantity: {Quantity}";
            description += $"\nType: {Beverage.Category}";
            description += $"\nBase Price: ₱{Beverage.BasePrice}";
            description += $"\nProduct ID: {Beverage.Code}";
            description += $"\nTotal Price: ₱{GetTotalPrice()}";

            return description;
        }

    }
}