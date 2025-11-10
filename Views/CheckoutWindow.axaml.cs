using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CoffeeShopPOS.Models;

namespace CoffeeShopPOS.Views
{
    public partial class CheckoutWindow : Window
    {
        // store order and calculation data
        private Order order;
        private Customer temporaryCustomer;  // for walk-in customers selecting discount type
        private decimal subtotal;
        private decimal customerDiscount;
        private decimal loyaltyDiscount;
        private decimal totalAmount;
        
        
        // constructor receives the order from OrderWindow
        public CheckoutWindow(Order order)
        {
            InitializeComponent();
            
            this.order = order;
            
            // Create temporary customer for walk-ins
            temporaryCustomer = new RegularCustomer("TEMP", "Walk-in");
            
            // Initialize display
            InitializeCheckout();
        }
        
        
        // method: Initialize the checkout display
        private void InitializeCheckout()
        {
            // Display order items
            DisplayOrderItems();
            
            // Display customer info
            DisplayCustomerInfo();
            
            // Calculate and display totals
            CalculateTotals();
        }
        
        
        // method: display all order items
        private void DisplayOrderItems()
        {
            var panel = this.FindControl<StackPanel>("OrderItemsPanel");
            if (panel == null) return;
            
            panel.Children.Clear();
            
            foreach (var item in order.GetItems())
            {
                // item description
                var itemText = new TextBlock
                {
                    Text = $"{item.Quantity}x {item.Beverage.Name} ({item.Size})",
                    FontWeight = Avalonia.Media.FontWeight.Bold
                };
                panel.Children.Add(itemText);
                
                // add-ons
                if (item.AddOns.Count > 0)
                {
                    foreach (var addon in item.AddOns)
                    {
                        var addonText = new TextBlock
                        {
                            Text = $"  + {addon.Name}",
                            FontSize = 12,
                            Foreground = Avalonia.Media.Brushes.Gray,
                            Margin = new Avalonia.Thickness(10, 0, 0, 0)
                        };
                        panel.Children.Add(addonText);
                    }
                }
                
                // price
                var priceText = new TextBlock
                {
                    Text = $"₱{item.GetTotalPrice():F2}",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    Margin = new Avalonia.Thickness(0, 0, 0, 10)
                };
                panel.Children.Add(priceText);
            }
        }
        
        
        // method: Display customer information
        private void DisplayCustomerInfo()
        {
            var txtCustomerInfo = this.FindControl<TextBlock>("TxtCustomerInfo");
            if (txtCustomerInfo == null) return;
            
            if (order.Customer != null)
            {
                // Logged-in customer
                txtCustomerInfo.Text = $"Customer: {order.Customer.Name} ({order.Customer.Type})";
                
                // Pre-select their customer type
                if (order.Customer is SeniorCustomer)
                    RbSenior.IsChecked = true;
                else if (order.Customer is PWDCustomer)
                    RbPWD.IsChecked = true;
                
                // Show loyalty panel if they have a loyalty account
                if (order.Customer.LoyaltyAccount != null)
                {
                    ShowLoyaltyPanel();
                }
            }
            else
            {
                // Walk-in customer
                txtCustomerInfo.Text = "Customer: Walk-in";
            }
        }
        
        
        // method: Show and setup loyalty panel
        private void ShowLoyaltyPanel()
        {
            var loyaltyPanel = this.FindControl<Border>("LoyaltyPanel");
            if (loyaltyPanel != null)
            {
                loyaltyPanel.IsVisible = true;
            }
            
            var loyaltyAccount = order.Customer?.LoyaltyAccount;
            if (loyaltyAccount == null) return;
            
            // Display available points
            var txtCurrentPoints = this.FindControl<TextBlock>("TxtCurrentPoints");
            if (txtCurrentPoints != null)
            {
                int maxRedeemable = loyaltyAccount.GetMaxRedeemablePoints();
                decimal discountValue = loyaltyAccount.GetDiscountValue(maxRedeemable);
                txtCurrentPoints.Text = $"Available: {loyaltyAccount.Points} points (Can redeem: {maxRedeemable} pts = ₱{discountValue:F2})";
            }
            
            // Set max redemption
            var numPointsToRedeem = this.FindControl<NumericUpDown>("NumPointsToRedeem");
            if (numPointsToRedeem != null)
            {
                numPointsToRedeem.Maximum = loyaltyAccount.GetMaxRedeemablePoints();
            }
        }
        
        
        // method: Calculate all totals
        private void CalculateTotals()
        {
            // Calculate subtotal
            subtotal = order.CalculateSubtotal();
            
            // Determine customer for discount calculation
            Customer customerForDiscount = order.Customer ?? temporaryCustomer;
            
            // Calculate customer discount
            customerDiscount = customerForDiscount.ApplyDiscount(subtotal);
            
            // Calculate loyalty discount
            loyaltyDiscount = 0;
            if (order.Customer?.LoyaltyAccount != null)
            {
                var numPointsToRedeem = this.FindControl<NumericUpDown>("NumPointsToRedeem");
                if (numPointsToRedeem != null)
                {
                    int pointsToRedeem = (int)(numPointsToRedeem.Value ?? 0);
                    loyaltyDiscount = order.Customer.LoyaltyAccount.GetDiscountValue(pointsToRedeem);
                }
            }
            
            // Calculate total
            totalAmount = subtotal - customerDiscount - loyaltyDiscount;
            
            // Update display
            UpdateTotalsDisplay();
        }
        
        
        // method: Update all total displays
        private void UpdateTotalsDisplay()
        {
            // Subtotal
            var txtSubtotal = this.FindControl<TextBlock>("TxtSubtotal");
            if (txtSubtotal != null)
                txtSubtotal.Text = $"₱{subtotal:F2}";
            
            // Customer discount
            var discountPanel = this.FindControl<Border>("DiscountPanel");
            var txtDiscount = this.FindControl<TextBlock>("TxtDiscount");
            if (discountPanel != null && txtDiscount != null)
            {
                if (customerDiscount > 0)
                {
                    discountPanel.IsVisible = true;
                    txtDiscount.Text = $"-₱{customerDiscount:F2}";
                }
                else
                {
                    discountPanel.IsVisible = false;
                }
            }
            
            // After discount
            var txtAfterDiscount = this.FindControl<TextBlock>("TxtAfterDiscount");
            if (txtAfterDiscount != null)
                txtAfterDiscount.Text = $"₱{(subtotal - customerDiscount):F2}";
            
            // Loyalty discount
            var loyaltyDiscountPanel = this.FindControl<Border>("LoyaltyDiscountPanel");
            var txtLoyaltyDiscount = this.FindControl<TextBlock>("TxtLoyaltyDiscount");
            if (loyaltyDiscountPanel != null && txtLoyaltyDiscount != null)
            {
                if (loyaltyDiscount > 0)
                {
                    loyaltyDiscountPanel.IsVisible = true;
                    txtLoyaltyDiscount.Text = $"-₱{loyaltyDiscount:F2}";
                }
                else
                {
                    loyaltyDiscountPanel.IsVisible = false;
                }
            }
            
            // Total
            var txtTotal = this.FindControl<TextBlock>("TxtTotal");
            if (txtTotal != null)
                txtTotal.Text = $"₱{totalAmount:F2}";
            
            // Recalculate change if payment entered
            UpdateChangeDisplay();
        }
        
        
        // event: Customer type changed
        private void CustomerType_Changed(object? sender, RoutedEventArgs e)
        {
            // Update temporary customer based on selection
            if (RbRegular?.IsChecked == true)
            {
                temporaryCustomer = new RegularCustomer("TEMP", "Walk-in");
            }
            else if (RbSenior?.IsChecked == true)
            {
                temporaryCustomer = new SeniorCustomer("TEMP", "Walk-in");
            }
            else if (RbPWD?.IsChecked == true)
            {
                temporaryCustomer = new PWDCustomer("TEMP", "Walk-in");
            }
            
            // Recalculate with new discount
            CalculateTotals();
        }
        
        
        // event: Points redemption changed
        private void PointsRedemption_Changed(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            // Recalculate with new loyalty discount
            CalculateTotals();
        }
        
        
        // event: Payment amount changed
        private void Payment_Changed(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            UpdateChangeDisplay();
        }
        
        
        // method: Update change display
        private void UpdateChangeDisplay()
        {
            var numPayment = this.FindControl<NumericUpDown>("NumPayment");
            var changePanel = this.FindControl<Border>("ChangePanel");
            var txtChange = this.FindControl<TextBlock>("TxtChange");
            var txtPaymentWarning = this.FindControl<TextBlock>("TxtPaymentWarning");
            var btnProcessPayment = this.FindControl<Button>("BtnProcessPayment");
            
            if (numPayment == null || changePanel == null || txtChange == null || 
                txtPaymentWarning == null || btnProcessPayment == null)
                return;
            
            // Safe way to get value - use ?? operator to provide default
            decimal payment = numPayment.Value ?? 0;  // ← Fixed!
            decimal change = payment - totalAmount;
            
            if (payment >= totalAmount && payment > 0)
            {
                // Sufficient payment
                changePanel.IsVisible = true;
                txtChange.Text = $"₱{change:F2}";
                txtPaymentWarning.IsVisible = false;
                btnProcessPayment.IsEnabled = true;
            }
            else if (payment > 0)
            {
                // Insufficient payment
                changePanel.IsVisible = false;
                txtPaymentWarning.IsVisible = true;
                btnProcessPayment.IsEnabled = false;
            }
            else
            {
                // No payment entered
                changePanel.IsVisible = false;
                txtPaymentWarning.IsVisible = false;
                btnProcessPayment.IsEnabled = false;
            }
        }
        
        
        // event: Process Payment button clicked
        private void BtnProcessPayment_Click(object? sender, RoutedEventArgs e)
        {
            // Get payment amount and change
            decimal payment = NumPayment?.Value ?? 0;
            decimal change = payment - totalAmount;
            
            // Redeem loyalty points if applicable
            int pointsRedeemed = 0;
            if (order.Customer?.LoyaltyAccount != null && loyaltyDiscount > 0)
            {
                pointsRedeemed = (int)(NumPointsToRedeem?.Value ?? 0);
                order.Customer.LoyaltyAccount.RedeemPoints(pointsRedeemed);
            }
            
            // Earn loyalty points if applicable
            int pointsEarned = 0;
            if (order.Customer?.LoyaltyAccount != null)
            {
                int pointsBefore = order.Customer.LoyaltyAccount.Points;
                order.Customer.LoyaltyAccount.EarnPoints(totalAmount);
                pointsEarned = order.Customer.LoyaltyAccount.Points - pointsBefore;
            }
            
            // Generate receipt
            ShowReceipt(payment, change, pointsEarned, pointsRedeemed);
        }
        
        
        // method: Show receipt
        private void ShowReceipt(decimal payment, decimal change, int pointsEarned, int pointsRedeemed)
        {
            string receipt = GenerateReceipt(payment, change, pointsEarned, pointsRedeemed);
            
            // Create receipt window
            var receiptWindow = new Window
            {
                Title = "Receipt",
                Width = 450,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            
            var scrollViewer = new ScrollViewer();
            var textBlock = new TextBlock
            {
                Text = receipt,
                FontFamily = "Courier New",
                Margin = new Avalonia.Thickness(20),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };
            
            scrollViewer.Content = textBlock;
            
            var stack = new StackPanel();
            stack.Children.Add(scrollViewer);
            
            var closeButton = new Button
            {
                Content = "Close",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Avalonia.Thickness(20),
                Width = 150,
                Height = 40
            };
            closeButton.Click += (s, e) =>
            {
                receiptWindow.Close();
                this.Close();  // Close checkout window too
            };
            stack.Children.Add(closeButton);
            
            receiptWindow.Content = stack;
            receiptWindow.ShowDialog(this);
        }
        
        
        // method: Generate receipt text
        private string GenerateReceipt(decimal payment, decimal change, int pointsEarned, int pointsRedeemed)
        {
            string receipt = "";
            receipt += "====================================\n";
            receipt += "   BREW & BEANS COFFEE SHOP\n";
            receipt += "   123 Main Street, Manila\n";
            receipt += "   Tel: (02) 1234-5678\n";
            receipt += "====================================\n";
            receipt += "      OFFICIAL RECEIPT\n";
            receipt += "\n";
            receipt += $"Date: {order.OrderDate:MMMM dd, yyyy}\n";
            receipt += $"Time: {order.OrderDate:h:mm tt}\n";
            receipt += $"Receipt No: {order.OrderId}\n";
            receipt += $"Cashier: Staff01\n";
            receipt += "\n";
            
            if (order.Customer != null)
            {
                receipt += $"Customer: {order.Customer.Name}\n";
                receipt += $"Customer ID: {order.Customer.Id}\n";
            }
            else
            {
                receipt += $"Customer: Walk-in\n";
            }
            
            receipt += "====================================\n";
            
            // Items
            foreach (var item in order.GetItems())
            {
                receipt += $"{item.Quantity}x {item.Beverage.Name} ({item.Size})\n";
                
                if (item.AddOns.Count > 0)
                {
                    foreach (var addon in item.AddOns)
                    {
                        receipt += $"  + {addon.Name}\n";
                    }
                }
                
                receipt += $"{"",30}₱{item.GetTotalPrice():F2}\n\n";
            }
            
            receipt += "------------------------------------\n";
            receipt += $"{"Subtotal:",30}₱{subtotal:F2}\n";
            
            if (customerDiscount > 0)
            {
                string discountType = temporaryCustomer.Type;
                if (order.Customer != null)
                    discountType = order.Customer.Type;
                
                receipt += $"{discountType + " Discount:",30}-₱{customerDiscount:F2}\n";
            }
            
            if (loyaltyDiscount > 0)
            {
                receipt += $"{"Loyalty Redemption:",30}-₱{loyaltyDiscount:F2}\n";
            }
            
            receipt += "------------------------------------\n";
            receipt += $"{"TOTAL AMOUNT:",30}₱{totalAmount:F2}\n";
            receipt += "------------------------------------\n";
            receipt += $"{"Cash:",30}₱{payment:F2}\n";
            receipt += $"{"Change:",30}₱{change:F2}\n";
            receipt += "\n";
            
            // Loyalty info
            if (order.Customer?.LoyaltyAccount != null)
            {
                receipt += "====================================\n";
                receipt += "        LOYALTY PROGRAM\n";
                receipt += "====================================\n";
                receipt += $"{"Points Earned:",30}+{pointsEarned} pts\n";
                
                if (pointsRedeemed > 0)
                {
                    receipt += $"{"Points Redeemed:",30}-{pointsRedeemed} pts\n";
                }
                
                receipt += "------------------------------------\n";
                receipt += $"{"New Balance:",30}{order.Customer.LoyaltyAccount.Points} pts\n";
                receipt += "\n";
                receipt += "Next Reward: 10 points = ₱50 OFF\n";
            }
            
            receipt += "====================================\n";
            receipt += "   Thank you for your purchase!\n";
            receipt += "      Please come again! ☕\n";
            receipt += "====================================\n";
            
            return receipt;
        }
        
        
        // event: Cancel button clicked
        private void BtnCancel_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}