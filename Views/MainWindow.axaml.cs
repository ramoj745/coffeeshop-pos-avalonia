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
        private Services.CustomerRepository customerRepository;

        public MainWindow()
        {
            // loads the XAML file
            InitializeComponent();

            // initialize customer repository
            customerRepository = new Services.CustomerRepository();
            // loads the created objects inside our list
            LoadSampleMenu();
            // displays the menu
            DisplayMenu();

            // create test customers if none exist
            CreateTestCustomers();

            // test CustomerRepository
            // TestCustomerRepository();

            // test TransactionLogger
            // TestTransactionLogger();

            // // Test addons
            // TestAddOns();
            // // Test Customer
            // TestCustomers();
            // // Test adding to order for customer
            // TestAddOrder();
            // // test order
            // TestOrder();
            // // Test LoyaltyAccount
            // TestLoyaltyAccount();
        }

        private void BtnNewOrder_Click(object? sender, RoutedEventArgs e)
        {
            var orderWindow = new OrderWindow();
            orderWindow.Show();
        }
        
        // create test customers
        private void CreateTestCustomers()
        {
            Console.WriteLine("\n=== CREATING TEST CUSTOMERS ===\n");
            
            // Check if customers already exist
            var existing = customerRepository.LoadAllCustomers();
            if (existing.Count > 0)
            {
                Console.WriteLine($"Found {existing.Count} existing customers. Skipping creation.");
                return;
            }
            
            // Create test customers
            var customer1 = new RegularCustomer(
                customerRepository.GenerateNewCustomerId(), 
                "Juan Dela Cruz"
            );
            customer1.LoyaltyAccount = new LoyaltyAccount(customer1.Id, 15);
            customerRepository.SaveCustomer(customer1);
            Console.WriteLine($"✓ Created: {customer1.Name} - ID: {customer1.Id}");
            
            var customer2 = new SeniorCustomer(
                customerRepository.GenerateNewCustomerId(),
                "Maria Santos"
            );
            customer2.LoyaltyAccount = new LoyaltyAccount(customer2.Id, 42);
            customerRepository.SaveCustomer(customer2);
            Console.WriteLine($"✓ Created: {customer2.Name} - ID: {customer2.Id}");
            
            var customer3 = new PWDCustomer(
                customerRepository.GenerateNewCustomerId(),
                "Pedro Reyes"
            );
            customer3.LoyaltyAccount = new LoyaltyAccount(customer3.Id, 8);
            customerRepository.SaveCustomer(customer3);
            Console.WriteLine($"✓ Created: {customer3.Name} - ID: {customer3.Id}");
            
            Console.WriteLine("\n✓ Test customers created! Use 'Customer Lookup' to find them.");
            Console.WriteLine("===================================\n");
        }


        // simple customer lookup
        private async void BtnCustomerLookup_Click(object? sender, RoutedEventArgs e)
        {
            // Create a simple input dialog
            var inputWindow = new Window
            {
                Title = "Customer Lookup",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            
            var stack = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 15
            };
            
            stack.Children.Add(new TextBlock
            {
                Text = "Enter Customer ID:",
                FontSize = 16,
                FontWeight = Avalonia.Media.FontWeight.Bold
            });
            
            var txtCustomerId = new TextBox
            {
                Watermark = "e.g., C00001",
                FontSize = 14
            };
            stack.Children.Add(txtCustomerId);
            
            var btnPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Spacing = 10,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Avalonia.Thickness(0, 20, 0, 0)
            };
            
            var btnLookup = new Button
            {
                Content = "Lookup",
                Width = 100,
                Height = 40
            };
            
            var btnCancel = new Button
            {
                Content = "Cancel",
                Width = 100,
                Height = 40
            };
            
            btnLookup.Click += (s, args) =>
            {
                string customerId = txtCustomerId.Text?.Trim() ?? "";
                
                if (string.IsNullOrEmpty(customerId))
                {
                    return;
                }
                
                // Load customer
                var customer = customerRepository.LoadCustomer(customerId);
                
                if (customer != null)
                {
                    Console.WriteLine($"✓ Customer found: {customer.Name}");
                    
                    // Open order window with this customer
                    var orderWindow = new OrderWindow(customer);
                    orderWindow.Show();
                    
                    inputWindow.Close();
                }
                else
                {
                    Console.WriteLine($"✗ Customer not found: {customerId}");
                    
                    // Show error message
                    var errorText = new TextBlock
                    {
                        Text = "Customer not found!",
                        Foreground = Avalonia.Media.Brushes.Red,
                        FontWeight = Avalonia.Media.FontWeight.Bold
                    };
                    
                    if (stack.Children.Count == 3)
                        stack.Children.RemoveAt(2);
                    
                    stack.Children.Insert(2, errorText);
                }
            };
            
            btnCancel.Click += (s, args) => inputWindow.Close();
            
            btnPanel.Children.Add(btnLookup);
            btnPanel.Children.Add(btnCancel);
            stack.Children.Add(btnPanel);
            
            inputWindow.Content = stack;
            await inputWindow.ShowDialog(this);
        }

        private void TestTransactionLogger()
        {
            Console.WriteLine("\n=== TESTING TRANSACTION LOGGER ===\n");
            
            var logger = new Services.TransactionLogger();
            
            // Test 1: Log some transactions
            Console.WriteLine("--- Logging Transactions ---");
            
            logger.LogTransaction(
                orderId: "ORD-20251106-001",
                customerId: "C00001",
                customerName: "Juan Dela Cruz",
                totalAmount: 362m,
                discountAmount: 0m,
                loyaltyRedeemed: 0m,
                pointsEarned: 7,
                pointsRedeemed: 0
            );
            
            logger.LogTransaction(
                orderId: "ORD-20251106-002",
                customerId: "C00002",
                customerName: "Maria Santos",
                totalAmount: 588m,
                discountAmount: 147m,
                loyaltyRedeemed: 50m,
                pointsEarned: 11,
                pointsRedeemed: 10
            );
            
            logger.LogTransaction(
                orderId: "ORD-20251106-003",
                customerId: "GUEST",
                customerName: "Walk-in Customer",
                totalAmount: 195m,
                discountAmount: 0m,
                loyaltyRedeemed: 0m,
                pointsEarned: 0,
                pointsRedeemed: 0
            );
            
            Console.WriteLine("✓ Logged 3 transactions");
            
            // Test 2: Load all transactions
            Console.WriteLine("\n--- Loading All Transactions ---");
            var allTransactions = logger.LoadAllTransactions();
            foreach (var t in allTransactions)
            {
                Console.WriteLine($"{t.DateTime:HH:mm} | {t.OrderId} | {t.CustomerName} | ₱{t.Amount:F2}");
            }
            
            // Test 3: Load transactions by date
            Console.WriteLine("\n--- Transactions for Today ---");
            var todayTransactions = logger.LoadTransactionsByDate(DateTime.Now);
            Console.WriteLine($"Found {todayTransactions.Count} transactions today");
            
            // Test 4: Calculate revenue
            Console.WriteLine("\n--- Revenue Summary ---");
            decimal totalRevenue = logger.GetTotalRevenueByDate(DateTime.Now);
            int transactionCount = logger.GetTransactionCountByDate(DateTime.Now);
            Console.WriteLine($"Total Transactions: {transactionCount}");
            Console.WriteLine($"Total Revenue: ₱{totalRevenue:F2}");
            
            if (transactionCount > 0)
            {
                decimal avgTransaction = totalRevenue / transactionCount;
                Console.WriteLine($"Average Transaction: ₱{avgTransaction:F2}");
            }
            
            // Test 5: Load transactions by customer
            Console.WriteLine("\n--- Transactions by Customer ---");
            var customerTransactions = logger.LoadTransactionsByCustomer("C00002");
            Console.WriteLine($"Maria Santos has {customerTransactions.Count} transaction(s):");
            foreach (var t in customerTransactions)
            {
                Console.WriteLine($"  {t.OrderId} - ₱{t.Amount:F2} ({t.PointsEarned} pts earned)");
            }
                        
            Console.WriteLine("====================================\n");
        }

        private void TestCustomerRepository()
        {
            Console.WriteLine("\n=== TESTING CUSTOMER REPOSITORY ===\n");

            var repo = new Services.CustomerRepository();

            // Test 1: Generate IDs
            Console.WriteLine("--- Generating Customer IDs ---");
            string id1 = repo.GenerateNewCustomerId();
            Console.WriteLine($"Generated ID: {id1}");

            // Test 2: Create and save customers
            Console.WriteLine("\n--- Creating Customers ---");

            var customer1 = new RegularCustomer(id1, "Juan Dela Cruz");
            customer1.LoyaltyAccount = new LoyaltyAccount(id1, 25);
            repo.SaveCustomer(customer1);

            string id2 = repo.GenerateNewCustomerId();
            var customer2 = new SeniorCustomer(id2, "Maria Santos");
            customer2.LoyaltyAccount = new LoyaltyAccount(id2, 47);
            repo.SaveCustomer(customer2);

            string id3 = repo.GenerateNewCustomerId();
            var customer3 = new PWDCustomer(id3, "Pedro Reyes");
            customer3.LoyaltyAccount = new LoyaltyAccount(id3, 12);
            repo.SaveCustomer(customer3);

            Console.WriteLine("✓ Saved 3 customers");

            // Test 3: Load all customers
            Console.WriteLine("\n--- Loading All Customers ---");
            var allCustomers = repo.LoadAllCustomers();
            foreach (var c in allCustomers)
            {
                Console.WriteLine($"{c.Id} - {c.Name} ({c.Type}) - {c.LoyaltyAccount?.Points ?? 0} points");
            }

            // Test 4: Load specific customer
            Console.WriteLine("\n--- Loading Specific Customer ---");
            var loadedCustomer = repo.LoadCustomer(id2);
            if (loadedCustomer != null)
            {
                Console.WriteLine($"Loaded: {loadedCustomer.Name}");
                Console.WriteLine($"Type: {loadedCustomer.Type}");
                Console.WriteLine($"Points: {loadedCustomer.LoyaltyAccount?.Points ?? 0}");
                Console.WriteLine($"Registered: {loadedCustomer.DateRegistered:yyyy-MM-dd}");
            }

            // Test 5: Update customer (add points)
            Console.WriteLine("\n--- Updating Customer Points ---");
            if (loadedCustomer?.LoyaltyAccount != null)
            {
                int oldPoints = loadedCustomer.LoyaltyAccount.Points;
                loadedCustomer.LoyaltyAccount.EarnPoints(250);  // Earn points from ₱250 purchase
                int newPoints = loadedCustomer.LoyaltyAccount.Points;

                Console.WriteLine($"Old points: {oldPoints}");
                Console.WriteLine($"Earned: {newPoints - oldPoints} points (from ₱250)");
                Console.WriteLine($"New points: {newPoints}");

                repo.SaveCustomer(loadedCustomer);
                Console.WriteLine("✓ Customer updated and saved");
            }

            // Test 6: Verify the update persisted
            Console.WriteLine("\n--- Verifying Persistence ---");
            var reloadedCustomer = repo.LoadCustomer(id2);
            if (reloadedCustomer?.LoyaltyAccount != null)
            {
                Console.WriteLine($"Reloaded points: {reloadedCustomer.LoyaltyAccount.Points}");
                Console.WriteLine("✓ Data persisted correctly!");
            }

            Console.WriteLine("====================================\n");
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
                new HotCoffee("C002", "Cappucino", 120),
                new IcedCoffee("C003", "Caramel Macchiato", 145),
                new IcedCoffee("C004", "Matcha Latte", 130),
                new BlendedCoffee("C005", "Mocha Frappucino", 165)
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