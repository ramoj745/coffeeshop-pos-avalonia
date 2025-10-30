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