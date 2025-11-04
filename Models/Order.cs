using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShopPOS.Models
{
    // represents a complete transaction/shopping cart
    // order has many OrderItems and has a Customer
    public class Order
    {
        // this is where we store our order items
        private List<OrderItem> items;

        // the customer making this order
        public Customer? Customer { get; set; }

        // when the order was created
        public DateTime OrderDate { get; private set; }

        // The order ID for this specific order
        public string OrderId { get; private set; } = string.Empty;

        // constructor
        public Order()
        {
            items = new List<OrderItem>();
            OrderDate = DateTime.Now;
            OrderId = GenerateOrderId();

        }

        // method: add items to the list
        public void AddItem(OrderItem item)
        {
            items.Add(item);
        }

        // method: remove items from the list

        public void RemoveItem(OrderItem item)
        {
            items.Remove(item);
        }

        // method: clear all items from the list

        public void ClearOrder()
        {
            items.Clear();
        }

        // Prevents outside code from modifying our list without going through our methods
        public List<OrderItem> GetItems()
        {
            return new List<OrderItem>(items);
        }

        // method: get the number of items in the order

        public int GetItemCount()
        {
            return items.Count;
        }

        // method: calculate subtotal of orderItem from list
        public decimal CalculateSubtotal()
        {
            return items.Sum(items => items.GetTotalPrice());
        }

        // method: calculate discount based on Customer Type
        public decimal CalculateCustomerDiscount()
        {
            if (Customer == null)
            {
                return 0;
            }

            // get subtotal
            decimal subtotal = CalculateSubtotal();

            // call Customer's ApplyDiscount method
            // different customer types return different discounts
            return Customer.ApplyDiscount(subtotal);
        }

        // method: calculate final total after discounts
        public decimal CalculateTotal()
        {
            decimal subtotal = CalculateSubtotal();
            decimal discount = CalculateCustomerDiscount();

            return subtotal - discount;
        }

        // method: calculate total with additional loyalty discount
        // overloaded method: same name, different parameters
        public decimal CalculateTotal(decimal loyaltyDiscount)
        {
            decimal subtotal = CalculateSubtotal();
            decimal customerDiscount = CalculateCustomerDiscount();

            return subtotal - customerDiscount - loyaltyDiscount;
        }

        // method: Get order summary as string (for display/debugging)
        public string GetOrderSummary()
        {
            string summary = $"Order ID: {OrderId}\n";
            summary += $"Date: {OrderDate:yyyy-MM-dd HH:mm:ss}\n";
            summary += $"Customer: {(Customer != null ? Customer.Name : "Walk-in")}\n";
            summary += $"Items: {items.Count}\n";
            summary += "\n--- ORDER ITEMS ---\n";

            foreach (var item in items)
            {
                summary += $"{item}\n";
            }

            summary += "\n--- TOTALS ---\n";
            summary += $"Subtotal: ₱{CalculateSubtotal():F2}\n";

            if (Customer != null)
            {
                decimal discount = CalculateCustomerDiscount();
                if (discount > 0)
                {
                    summary += $"Discount ({Customer.Type}): -₱{discount:F2}\n";
                }
            }

            summary += $"TOTAL: ₱{CalculateTotal():F2}\n";

            return summary;
        }
        
        // helper method: generate unique order ID
        private string GenerateOrderId()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int randomPart = new Random().Next(99, 100);

            return $"ORD-{datePart}-{randomPart}";
        }


    }
}