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
        }
        
        private void TestAddOns()
        {
            Console.WriteLine("\nTesting Addons");
            AddOn extraShot = new AddOn("Extra Shot", 25);
            Console.WriteLine(extraShot);
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