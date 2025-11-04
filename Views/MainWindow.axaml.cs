using CoffeeShopPOS.Models;
using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CoffeeShopPOS.Views
{
    public partial class MainWindow : Window
    {
        private List<Beverage> menuItems = new List<Beverage>();

        public MainWindow()
        {
            // loads the XAML file
            InitializeComponent();
            // loads the created objects inside our list
            LoadSampleMenu();
            // displays the menu
            DisplayMenu();
            // Test addons
            TestAddOns();
            // Test Customer
            TestCustomers();
            // Test adding to order for customer
            TestAddOrder();
            // test order
            TestOrder();
            // Test LoyaltyAccount
            TestLoyaltyAccount();
        }

        private void TestLoyaltyAccount()
        {
            Customer customer = new PWDCustomer("C001", "Ramoj");
            customer.LoyaltyAccount = new LoyaltyAccount(customer.Id);

            Console.WriteLine($"Customer: {customer.Name}");
            Console.WriteLine(customer.LoyaltyAccount);
            Console.WriteLine();

            Console.WriteLine("=== EARNING POINTS ===");
            decimal amount1 = 1064m; // sample paying amount

            customer.LoyaltyAccount.EarnPoints(amount1);

            Console.WriteLine($"Spent ₱{amount1:F2} → Earned {(int)(amount1 / 50)} points");
            Console.WriteLine($"Balance: {customer.LoyaltyAccount.Points} points");
            Console.WriteLine();

            // Test redemption info
            Console.WriteLine("=== REDEMPTION INFO ===");
            int maxRedeemable = customer.LoyaltyAccount.GetMaxRedeemablePoints();
            decimal discountValue = customer.LoyaltyAccount.GetDiscountValue(maxRedeemable);
            Console.WriteLine($"Can redeem: {maxRedeemable} points");
            Console.WriteLine($"Discount value: ₱{discountValue:F2}");
            Console.WriteLine($"Points to next reward: {customer.LoyaltyAccount.GetPointsToNextReward()}");
            Console.WriteLine();

            // Test redeeming points - VALID
            Console.WriteLine("=== REDEEMING 10 POINTS ===");
            bool success = customer.LoyaltyAccount.RedeemPoints(10);
            Console.WriteLine($"Redemption success: {success}");
            Console.WriteLine($"New balance: {customer.LoyaltyAccount.Points} points");
            Console.WriteLine($"Discount received: ₱{customer.LoyaltyAccount.GetDiscountValue(10):F2}");
            Console.WriteLine();

            // Try to redeem 100 points (more than balance)
            Console.WriteLine("Trying to redeem 100 points...");
            success = customer.LoyaltyAccount.RedeemPoints(100);
            Console.WriteLine($"Result: {(success ? "Success" : "Failed - insufficient points")}");
            Console.WriteLine($"Current balance: {customer.LoyaltyAccount.Points} points");
            Console.WriteLine();

            // Test with CanRedeem method
            Console.WriteLine("=== CHECKING REDEMPTION VALIDITY ===");
            Console.WriteLine($"Can redeem 10? {customer.LoyaltyAccount.CanRedeem(10)}");
            Console.WriteLine($"Can redeem 5? {customer.LoyaltyAccount.CanRedeem(5)}");
            Console.WriteLine($"Can redeem 100? {customer.LoyaltyAccount.CanRedeem(100)}");
            Console.WriteLine();

            // Complete transaction example with order
            Console.WriteLine("=== COMPLETE TRANSACTION WITH LOYALTY ===");
            
            // Create an order
            Order order = new Order();
            order.Customer = customer;
            
            Beverage coffee = new HotCoffee("C001", "Americano", 95);
            OrderItem item = new OrderItem(coffee, "Medium", 3);
            order.AddItem(item);
            
            // Calculate totals
            decimal subtotal = order.CalculateSubtotal();
            decimal customerDiscount = order.CalculateCustomerDiscount();
            
            Console.WriteLine($"Subtotal: ₱{subtotal:F2}");
            Console.WriteLine($"Customer Discount: -₱{customerDiscount:F2}");
            
            // Redeem loyalty points
            int pointsToRedeem = 0;  // Customer's current balance is only 4 now
            if (customer.LoyaltyAccount.CanRedeem(10))
            {
                // This won't execute because balance is only 4 points
                pointsToRedeem = 10;
                customer.LoyaltyAccount.RedeemPoints(pointsToRedeem);
            }
            
            decimal loyaltyDiscount = customer.LoyaltyAccount.GetDiscountValue(pointsToRedeem);
            Console.WriteLine($"Loyalty Discount: -₱{loyaltyDiscount:F2}");
            
            decimal total = subtotal - customerDiscount - loyaltyDiscount;
            Console.WriteLine($"Total: ₱{total:F2}");
            Console.WriteLine();
            
            // Earn points from this purchase
            int pointsBefore = customer.LoyaltyAccount.Points;
            customer.LoyaltyAccount.EarnPoints(total);
            int pointsEarned = customer.LoyaltyAccount.Points - pointsBefore;
            
            Console.WriteLine($"Points earned: +{pointsEarned}");
            Console.WriteLine($"New balance: {customer.LoyaltyAccount.Points} points");
            
            Console.WriteLine("\n==============================\n");

        }

        private void TestOrder()
        {
            Console.WriteLine("\nTesting Order Class");

            // create our customer
            Customer customer = new SeniorCustomer("C001", "Maria Santos");
            // Create Order
            Order order = new Order();

            // set created customer to order
            order.Customer = customer;

            Console.WriteLine($"Created Order ID: {order.OrderId}");
            Console.WriteLine();

            // add items to the order
            Beverage caramelMacchiato = new IcedCoffee("C003", "Caramel Macchiato", 145);
            OrderItem item1 = new OrderItem(caramelMacchiato, "Medium", 4);

            item1.AddAddOn(new AddOn("Extra Shot", 25));

            // add the item to the order
            order.AddItem(item1);

            Console.WriteLine(order.GetOrderSummary());
            Console.WriteLine();

            // Calculation breakdown
            Console.WriteLine("\n ORDER CALCULATION SUMMARY");

            decimal subtotal = order.CalculateSubtotal();
            decimal discount = order.CalculateCustomerDiscount();
            decimal total = order.CalculateTotal();

            Console.WriteLine($"Subtotal: ₱{subtotal:F2}");
            Console.WriteLine($"Customer Type: {customer.Type}");
            Console.WriteLine($"Discount (20%): -₱{discount:F2}");
            Console.WriteLine($"Final Total: ₱{total:F2}");
            Console.WriteLine();
            Console.WriteLine($"With Loyalty Discount: ₱{order.CalculateTotal(70)}");

            Console.WriteLine($"{order.GetOrderSummary()}");


        }

        private void TestAddOrder()
        {
            Console.WriteLine("\nTesting OrderItem");
            Console.WriteLine("\n Customer 1:");
            Customer ramoj = new PWDCustomer("C001", "Ramoj Mabilangan");

            decimal money = 900m;

            Console.WriteLine($"\n For Customer: {ramoj.Name}");
            Console.WriteLine($"\n Money: {money}");
            Console.WriteLine($"\n Customer Type: {ramoj.Type}");

            Beverage caramelMacchiato = new IcedCoffee("C003", "Caramel Macchiato", 120);

            OrderItem item1 = new OrderItem(caramelMacchiato, "Medium", 3);

            item1.AddAddOn(new AddOn("Extra Shot", 25));
            item1.AddAddOn(new AddOn("Whipped Cream", 30));

            Console.WriteLine("\n Order Item 1:");
            Console.WriteLine($"\n {item1.GetDescription()}");

            decimal discount = ramoj.ApplyDiscount(item1.GetTotalPrice());

            Console.WriteLine($"\n Discount: {discount}");
            Console.WriteLine($"\n After Discount: {item1.GetTotalPrice() - discount}");

        }
        private void TestAddOns()
        {
            Console.WriteLine("\nTesting Addons");
            AddOn extraShot = new AddOn("Extra Shot", 25);
            Console.WriteLine(extraShot);
        }
        
        public void TestCustomers()
        {
            Console.WriteLine("\n Testing Customers");
            Customer pwd = new PWDCustomer("C001", "Juan");

            decimal amount = 500m;

            Console.WriteLine($"Order Amount: ₱{amount:F2}\n");
            Console.WriteLine(pwd);
            decimal regularDiscount = pwd.ApplyDiscount(amount);
            Console.WriteLine($"Discount: ₱{regularDiscount:F2}");
            Console.WriteLine($"Final: ₱{amount - regularDiscount:F2}\n");

            List<Customer> customers = new List<Customer>
            {
                new RegularCustomer("C004", "Ana Lopez"),
                new PWDCustomer("C005", "Ramoj Mabilangan"),
                new SeniorCustomer("C006", "Tin Solis")
            };

            foreach(Customer customer in customers)
            {
                decimal discount = customer.ApplyDiscount(amount);
                Console.WriteLine($"{customer.Name} ({customer.Type}) - {customer.Id} : Discount {discount}");
            }

        }
        
        private void LoadSampleMenu()
        {
            menuItems = new List<Beverage>
            {
                new HotCoffee("C001", "Americano", 95),
                new HotCoffee("C002", "Cappuccino", 120),
                new IcedCoffee("C003", "Caramel Macchiato", 145),
                new IcedCoffee("C004", "Iced Latte", 130),
                new BlendedCoffee("C005", "Mocha Frappuccino", 165)
            };

            Console.WriteLine($"Menu loaded with {menuItems.Count} items");

        }
        
        private void DisplayMenu()
        {
            // Find ItemsControl in XAML
            var menuControl = this.FindControl<ItemsControl>("MenuItemsControl");
            
            if (menuControl == null)
            {
                Console.WriteLine("Error: MenuItemsControl not found!");
                return;
            }
            
            // Set the items
            menuControl.ItemsSource = menuItems;
            
            Console.WriteLine($"Displayed {menuItems.Count} menu items");
            
            // Debug output
            foreach (var item in menuItems)
            {
                Console.WriteLine($"  {item.Code} - {item.Name} - {item.Category} - ₱{item.BasePrice}");
            }
        }
        
        private void BtnRefreshMenu_Click(object? sender, RoutedEventArgs e)
        {
            DisplayMenu();
            Console.WriteLine("Menu refreshed!");
        }
        
        private void BtnExit_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}