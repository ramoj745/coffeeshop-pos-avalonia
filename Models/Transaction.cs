using System;

namespace CoffeeShopPOS.Models
{
    // transaction represents a completed sale record
    public class Transaction
    {
        public DateTime DateTime { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        // financial details
        public decimal Amount { get; set; } // final total paid
        public decimal DiscountAmount { get; set; } // customer discount from order
        public decimal LoyaltyRedeemed { get; set; } // loyalty discount applied

        // loyalty points info
        public int PointsEarned { get; set; }
        public int PointsRedeemed { get; set; }

        // constructor (empty for deserialization)
        public Transaction()
        {
        }
        // all data
        public Transaction(string orderId, string customerId, string customerName,
                          decimal amount, decimal discountAmount, decimal loyaltyRedeemed,
                          int pointsEarned, int pointsRedeemed)
        {
            DateTime = DateTime.Now;
            OrderId = orderId;
            CustomerId = customerId;
            CustomerName = customerName;
            Amount = amount;
            DiscountAmount = discountAmount;
            LoyaltyRedeemed = loyaltyRedeemed;
            PointsEarned = pointsEarned;
            PointsRedeemed = pointsRedeemed;
        }

        // method: convert to log string 
        // format: DateTime|OrderID|CustomerID|CustomerName|Amount|Discount|LoyaltyRedeemed|PointsEarned|PointsRedeemed
        public string ToLogString()
        {
            return $"{DateTime:yyyy-MM-dd HH:mm:ss}|{OrderId}|{CustomerId}|{CustomerName}|" +
                   $"{Amount:F2}|{DiscountAmount:F2}|{LoyaltyRedeemed:F2}|" +
                   $"{PointsEarned}|{PointsRedeemed}";
        }

        // for debugging
        public override string ToString()
        {
            return $"{DateTime:yyyy-MM-dd HH:mm} | {OrderId} | {CustomerName} | ₱{Amount:F2}";
        }
    }
}