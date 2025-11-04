using System;

namespace CoffeeShopPOS.Models
{
    // This class manages a customer's loyalty points
    public class LoyaltyAccount
    {
        private int points;

        // Customer ID this account belongs to
        public string CustomerId { get; private set; } = string.Empty;

        // points balance
        public int Points
        {
            get
            {
                return points;
            }
            set
            {
                // prevent negative points
                if (value < 0)
                    throw new ArgumentException("Points cannot be negative.");

                points = value;
            }
        }

        // creates loyalty account for customer
        public LoyaltyAccount(string customerId)
        {
            CustomerId = customerId;
            Points = 0;
        }

        // create with existing points
        public LoyaltyAccount(string customerId, int initalPoints)
        {
            CustomerId = customerId;
            Points = initalPoints; // uses setter with validation
        }

        // method: earn points based on amount spent
        // rule: 1 point per 50 pesos spent (rounded down!)
        public void EarnPoints(decimal amountSpent)
        {
            // valid inputs first
            if (amountSpent < 0)
                throw new ArgumentException("Amount spent cannot be negative.");

            // calculate points earned
            int earnedPoints = (int)(amountSpent / 50);

            // add to current point balance
            Points += earnedPoints;
        }

        // method: redeem points for discount
        // rule: must redeem in multiples of 10

        public bool RedeemPoints(int pointsToRedeem)
        {
            // validation 1: must be multiple of 10
            if (pointsToRedeem % 10 != 0)
            {
                return false; // failed
            }

            if (pointsToRedeem > Points)
            {
                return false; // failed, insufficient points
            }

            if (pointsToRedeem <= 0)
            {
                return false; // failed - invalid amount
            }

            // finally, if passed all checks, subtract from points
            Points -= pointsToRedeem;
            return true; // success
        }

        // method: calculate discount value from points
        public decimal GetDiscountValue(int pointsToRedeem)
        {
            if (pointsToRedeem < 0)
            {
                return 0;
            }

            // example: (25 points ÷ 10) * 50 = 2.5 * 50 = ₱125
            return (pointsToRedeem / 10) * 50;
        }

        // method: get maxixmum redeemable points
        // returns the highest multiple of 10 that can be redeemedd
        // 47 points : 40 points can be redeemed
        public int GetMaxRedeemablePoints()
        {
            return (Points / 10) * 10;
        }

        // method: check if customer can redeem a certain amount
        public bool CanRedeem(int pointsToRedeem)
        {
            return pointsToRedeem > 0 && pointsToRedeem % 10 == 0 && pointsToRedeem <= Points;
        }

        // method get points needed for next reward
        // ex. if customer has 47 points, needs 3 more to reach 50
        public int GetPointsToNextReward()
        {
            int currentRedeemable = GetMaxRedeemablePoints();
            int nextReward = currentRedeemable + 10;

            return nextReward - Points;
        }

        // for debugging
        public override string ToString()
        {
            int redeemable = GetMaxRedeemablePoints();
            decimal discountValue = GetDiscountValue(redeemable);
            
            return $"Loyalty Account [{CustomerId}]: {Points} points " +
                   $"(Can redeem: {redeemable} pts = ₱{discountValue:F2})";
        }

        
    }
}