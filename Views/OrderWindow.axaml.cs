using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using CoffeeShopPOS.Models;
using Microsoft.VisualBasic;

namespace CoffeeShopPOS.Views
{
    public partial class OrderWindow : Window
    {
        // store states
        private List<Beverage> menuItems; // available menu items
        private Order currentOrder; // the order being build
        private Beverage? selectedBeverage; // currently selected beverage


        // constructor
        public OrderWindow()
        {
            InitializeComponent();

            // initialize data
            LoadMenu();
            currentOrder = new Order();

            // Display menu
            DisplayMenu();

            // Update UI
            UpdateOrderDisplay();

        }

        public OrderWindow(Customer customer) : this()
        {
            // set customer for this order
            currentOrder.Customer = customer;

            // update customer display
            var txtCustomerInfo = this.FindControl<TextBlock>("TxtCustomerInfo");
            if (txtCustomerInfo != null)
            {
                txtCustomerInfo.Text = $"Customer: {customer.Name} ({customer.Type})";
            }
        }

        // method: load menu items
        private void LoadMenu()
        {
            menuItems = new List<Beverage>
            {
                new HotCoffee("C001", "Americano", 95),
                new HotCoffee("C002", "Cappucino", 120),
                new IcedCoffee("C003", "Caramel Macchiato", 145),
                new IcedCoffee("C004", "Matcha Latte", 130),
                new BlendedCoffee("C005", "Mocha Frappucino", 165)
            };
        }

        // method: display menu in ItemsControl
        private void DisplayMenu()
        {
            Console.WriteLine($"DisplayMenu called. Menu has {menuItems.Count} items");
            var menuControl = this.FindControl<ItemsControl>("MenuItemsControl");
            if (menuControl != null)
            {
                menuControl.ItemsSource = menuItems;
            }
        }

        // event: when a menu item is clicked
        private void MenuItem_Tapped(object? sender, PointerPressedEventArgs e)
        {
            // get border that was clicked
            if (sender is Border border)
            {
                // get the beverage from the border's DataContext
                // DataContext is automatically set by the ItemsControl
                if (border.DataContext is Beverage beverage)
                {
                    SelectBeverage(beverage);
                }
            }
        }

        // method: select a beverage for customization
        private void SelectBeverage(Beverage beverage)
        {
            selectedBeverage = beverage;

            // show customziation panel
            var customizationPanel = this.FindControl<Border>("CustomizationPanel");
            if (customizationPanel != null)
            {
                customizationPanel.IsVisible = true;
            }

            // update the selected item text
            var txtSelectedItem = this.FindControl<TextBlock>("TxtSelectedItem");
            if (txtSelectedItem != null)
            {
                txtSelectedItem.Text = $"Selected: {beverage.Name} (₱{beverage.BasePrice:F2})";
            }

            // enable add button
            var btnAddToOrder = this.FindControl<Button>("BtnAddToOrder");
            if (btnAddToOrder != null)
            {
                btnAddToOrder.IsEnabled = true;
            }

            Console.WriteLine($"Selected: {beverage.Name}");
        }

        // event: Add to Order button clicked
        private void BtnAddToOrder_Click(object? sender, RoutedEventArgs e)
        {
            if (selectedBeverage == null)
            {
                return;
            }

            // get selected size
            string size = "Small"; // set small as default

            if (RbMedium?.IsChecked == true)
            {
                size = "Medium";
            }
            else if (RbLarge?.IsChecked == true)
            {
                size = "Large";
            }

            // get quantity
            int quantity = (int)(NumQuantity?.Value ?? 1);

            // create OrderItem
            OrderItem orderItem = new OrderItem(selectedBeverage, size, quantity);

            // add selected add-ons
            if (ChkExtraShot?.IsChecked == true)
                orderItem.AddAddOn(new AddOn("Extra Shot", 25));

            if (ChkWhippedCream?.IsChecked == true)
                orderItem.AddAddOn(new AddOn("Whipped Cream", 15));

            if (ChkCaramelDrizzle?.IsChecked == true)
                orderItem.AddAddOn(new AddOn("Caramel Drizzle", 20));

            // add to order
            currentOrder.AddItem(orderItem);

            Console.WriteLine($"Added Item: {orderItem}");

            // Update display
            UpdateOrderDisplay();

            // Reset customization
            ResetCustomization();
        }

        // method: reset customization panel
        private void ResetCustomization()
        {
            // reset size to Small
            if (RbSmall != null)
                RbSmall.IsChecked = true;

            // uncheck all add-ons
            if (ChkExtraShot != null)
                ChkExtraShot.IsChecked = false;
            if (ChkWhippedCream != null)
                ChkWhippedCream.IsChecked = false;
            if (ChkCaramelDrizzle != null)
                ChkCaramelDrizzle.IsChecked = false;

            // reset quantity
            if (NumQuantity != null)
                NumQuantity.Value = 1;

            // clear selection
            selectedBeverage = null;

            // hide customization panel
            var customizationPanel = this.FindControl<Border>("CustomizationPanel");
            if (customizationPanel != null)
            {
                customizationPanel.IsVisible = false;
            }

            // disable add button
            var btnAddToOrder = this.FindControl<Button>("BtnAddToOrder");
            if (btnAddToOrder != null)
            {
                btnAddToOrder.IsEnabled = false;
            }
        }

        // method: update the order display (cart)
        private void UpdateOrderDisplay()
        {
            var orderItemsPanel = this.FindControl<StackPanel>("OrderItemsPanel");
            if (orderItemsPanel == null)
                return;

            // clear existing items
            orderItemsPanel.Children.Clear();

            // get all items from order
            var items = currentOrder.GetItems();

            if (items.Count == 0)
            {
                // Show 'no items' message
                orderItemsPanel.Children.Add(new TextBlock
                {
                    Text = "No items in order",
                    Foreground = Avalonia.Media.Brushes.Gray,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                });
            }
            else
            {
                // display each item
                foreach (var item in items)
                {
                    var itemBorder = CreateOrderItemDisplay(item);
                    orderItemsPanel.Children.Add(itemBorder);
                }
            }
            // update totals
            UpdateTotals();

        }

        // method: create UI for one order item
        private Border CreateOrderItemDisplay(OrderItem item)
        {
            var stackPanel = new StackPanel { Spacing = 5 };

            // item description
            var nameText = new TextBlock
            {
                Text = $"{item.Quantity}x {item.Beverage.Name} ({item.Size})",
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = Avalonia.Media.Brushes.Black
            };
            stackPanel.Children.Add(nameText);

            // add-ons
            if (item.AddOns.Count > 0)
            {
                foreach (var addon in item.AddOns)
                {
                    var addonText = new TextBlock
                    {
                        Text = $"  + {addon.Name}",
                        FontSize = 12,
                        Foreground = Avalonia.Media.Brushes.Gray
                    };
                    stackPanel.Children.Add(addonText);
                };
            }

            // price
            var priceText = new TextBlock
            {
                Text = $"₱{item.GetTotalPrice():F2}",
                FontWeight = Avalonia.Media.FontWeight.Bold,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
            };
            stackPanel.Children.Add(priceText);

            // wrap in border
            return new Border
            {
                Child = stackPanel,
                Padding = new Avalonia.Thickness(10),
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(249, 249, 249)),
                CornerRadius = new Avalonia.CornerRadius(5)
            };
        }

        // method: update totals display
        private void UpdateTotals()
        {
            decimal subtotal = currentOrder.CalculateSubtotal();
            int itemCount = currentOrder.GetItemCount();

            // update subtotal
            var txtSubtotal = this.FindControl<TextBlock>("TxtSubtotal");
            if (txtSubtotal != null)
            {
                txtSubtotal.Text = $"₱{subtotal:F2}";
                txtSubtotal.Foreground = Avalonia.Media.Brushes.Black;
            }

            // update item count
            var txtItemCount = this.FindControl<TextBlock>("TxtItemCount");
            if (txtItemCount != null)
            {
                txtItemCount.Text = itemCount.ToString();
                txtItemCount.Foreground = Avalonia.Media.Brushes.Black;
            }

            // Enable/disable checkout button
            var btnCheckout = this.FindControl<Button>("BtnCheckout");
            if (btnCheckout != null)
            {
                btnCheckout.IsEnabled = itemCount > 0;
            }
        }

        // event: checkout button clicked
        private void BtnCheckout_Click(object? sender, RoutedEventArgs e)
        {
            // TODO: Open CheckoutWindow
            Console.WriteLine("Proceeding to checkout...");
            Console.WriteLine(currentOrder.GetOrderSummary());

            // For now, just show a message
            var messageBox = new Window
            {
                Title = "Checkout",
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = "Checkout window coming soon!\n\nCheck console for order summary.",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    TextAlignment = Avalonia.Media.TextAlignment.Center
                }
            };
            messageBox.ShowDialog(this);
        }

        // event: clear order button clicked
        private void BtnClearOrder_Click(object? sender, RoutedEventArgs e)
        {
            currentOrder.ClearOrder();
            UpdateOrderDisplay();
            Console.WriteLine("Order cleared");
        }

        // event: cancel button clicked
        private void BtnCancel_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        

        






        

    }
}