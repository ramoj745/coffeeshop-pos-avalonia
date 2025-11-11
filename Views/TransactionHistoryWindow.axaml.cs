using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Avalonia;                          
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using CoffeeShopPOS.Models;
using CoffeeShopPOS.Services;

namespace CoffeeShopPOS.Views
{
    public partial class TransactionHistoryWindow : Window
    {
        private TransactionLogger transactionLogger;
        private List<Transaction> currentTransactions;
        
        
        // constructor
        public TransactionHistoryWindow()
        {
            InitializeComponent();
            
            transactionLogger = new TransactionLogger();
            currentTransactions = new List<Transaction>();
            
            // load initial data (today)
            LoadTransactions();
        }
        
        
        // method: Load transactions based on selected period
        private void LoadTransactions()
        {
            // Determine which period is selected
            if (RbToday?.IsChecked == true)
            {
                currentTransactions = transactionLogger.LoadTransactionsByDate(DateTime.Now);
                Console.WriteLine($"Loading today's transactions: {currentTransactions.Count}");
            }
            else if (RbWeek?.IsChecked == true)
            {
                currentTransactions = GetThisWeekTransactions();
                Console.WriteLine($"Loading this week's transactions: {currentTransactions.Count}");
            }
            else if (RbMonth?.IsChecked == true)
            {
                currentTransactions = GetThisMonthTransactions();
                Console.WriteLine($"Loading this month's transactions: {currentTransactions.Count}");
            }
            else if (RbAll?.IsChecked == true)
            {
                currentTransactions = transactionLogger.LoadAllTransactions();
                Console.WriteLine($"Loading all transactions: {currentTransactions.Count}");
            }
            
            // Update displays
            UpdateSummaryCards();
            DisplayTransactions();
        }
        
        
        // method: Get transactions for this week (Monday to today)
        private List<Transaction> GetThisWeekTransactions()
        {
            var allTransactions = transactionLogger.LoadAllTransactions();
            
            // Find the start of this week (Monday)
            DateTime today = DateTime.Now.Date;
            int daysFromMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime startOfWeek = today.AddDays(-daysFromMonday);
            
            return allTransactions
                .Where(t => t.DateTime.Date >= startOfWeek && t.DateTime.Date <= today)
                .ToList();
        }
        
        
        // method: Get transactions for this month
        private List<Transaction> GetThisMonthTransactions()
        {
            var allTransactions = transactionLogger.LoadAllTransactions();
            
            DateTime today = DateTime.Now.Date;
            int currentMonth = today.Month;
            int currentYear = today.Year;
            
            return allTransactions
                .Where(t => t.DateTime.Month == currentMonth && t.DateTime.Year == currentYear)
                .ToList();
        }
        
        
        // method: Update summary cards with calculations
        private void UpdateSummaryCards()
        {
            // Calculate totals
            decimal totalRevenue = currentTransactions.Sum(t => t.Amount);
            int transactionCount = currentTransactions.Count;
            decimal avgTransaction = transactionCount > 0 ? totalRevenue / transactionCount : 0;
            decimal totalDiscounts = currentTransactions.Sum(t => t.DiscountAmount + t.LoyaltyRedeemed);
            
            // Update UI
            if (TxtTotalRevenue != null)
                TxtTotalRevenue.Text = $"₱{totalRevenue:F2}";
            
            if (TxtTransactionCount != null)
                TxtTransactionCount.Text = transactionCount.ToString();
            
            if (TxtAvgTransaction != null)
                TxtAvgTransaction.Text = $"₱{avgTransaction:F2}";
            
            if (TxtTotalDiscounts != null)
                TxtTotalDiscounts.Text = $"₱{totalDiscounts:F2}";
        }
        
        
        // method: Display transactions in the list
        private void DisplayTransactions()
        {
            var panel = this.FindControl<StackPanel>("TransactionListPanel");
            if (panel == null) return;
            
            panel.Children.Clear();
            
            if (currentTransactions.Count == 0)
            {
                // Show "no transactions" message
                panel.Children.Add(new TextBlock
                {
                    Text = "No transactions found for this period",
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0),
                    FontSize = 16
                });
                return;
            }
            
            // Sort by date (newest first)
            var sortedTransactions = currentTransactions.OrderByDescending(t => t.DateTime).ToList();
            
            // Display each transaction
            foreach (var transaction in sortedTransactions)
            {
                var transactionBorder = CreateTransactionDisplay(transaction);
                panel.Children.Add(transactionBorder);
            }
        }
        
        
        // method: Create UI element for one transaction
        private Border CreateTransactionDisplay(Transaction transaction)
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("140,160,*,100,100,80")
            };
            
            // Date/Time
            var dateText = new TextBlock
            {
                Text = transaction.DateTime.ToString("MM/dd HH:mm"),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Black
            };
            Grid.SetColumn(dateText, 0);
            grid.Children.Add(dateText);
            
            // Order ID
            var orderText = new TextBlock
            {
                Text = transaction.OrderId,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Courier New"),
                Foreground = Brushes.Black
            };
            Grid.SetColumn(orderText, 1);
            grid.Children.Add(orderText);
            
            // Customer
            var customerText = new TextBlock
            {
                Text = transaction.CustomerName,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = Avalonia.Media.TextTrimming.CharacterEllipsis,
                Foreground = Brushes.Black
            };
            Grid.SetColumn(customerText, 2);
            grid.Children.Add(customerText);
            
            // Amount
            var amountText = new TextBlock
            {
                Text = $"₱{transaction.Amount:F2}",
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.Green
            };
            Grid.SetColumn(amountText, 3);
            grid.Children.Add(amountText);
            
            // Discount
            decimal totalDiscount = transaction.DiscountAmount + transaction.LoyaltyRedeemed;
            var discountText = new TextBlock
            {
                Text = totalDiscount > 0 ? $"-₱{totalDiscount:F2}" : "-",
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = totalDiscount > 0 ? Brushes.OrangeRed : Brushes.Gray
            };
            Grid.SetColumn(discountText, 4);
            grid.Children.Add(discountText);
            
            // Points
            var pointsText = new TextBlock
            {
                Text = transaction.PointsEarned > 0 ? $"+{transaction.PointsEarned}" : "-",
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = transaction.PointsEarned > 0 ? Brushes.Blue : Brushes.Gray
            };
            Grid.SetColumn(pointsText, 5);
            grid.Children.Add(pointsText);
            
            // Wrap in border
            return new Border
            {
                Child = grid,
                Padding = new Thickness(10, 8),
                Background = new SolidColorBrush(Color.FromRgb(249, 249, 249)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
        }
        
        
        // EVENT: Period selection changed
        private void Period_Changed(object? sender, RoutedEventArgs e)
        {
            LoadTransactions();
        }
        
        
        // EVENT: Refresh button clicked
        private void BtnRefresh_Click(object? sender, RoutedEventArgs e)
        {
            LoadTransactions();
            Console.WriteLine("Transaction history refreshed");
        }
        
        
        // EVENT: Export button clicked
        private void BtnExport_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Determine period name for filename
                string periodName = "all";
                if (RbToday?.IsChecked == true)
                    periodName = "today";
                else if (RbWeek?.IsChecked == true)
                    periodName = "week";
                else if (RbMonth?.IsChecked == true)
                    periodName = "month";
                
                // Generate filename
                string filename = $"transaction_report_{periodName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                
                // Generate report content
                string report = GenerateReport();
                
                // Write to file
                File.WriteAllText(filename, report);
                
                Console.WriteLine($"Report exported to: {filename}");
                
                // Show success message
                ShowMessage("Export Successful", $"Report saved to:\n{filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting report: {ex.Message}");
                ShowMessage("Export Failed", $"Error: {ex.Message}");
            }
        }
        
        
        // method: Generate text report
        private string GenerateReport()
        {
            string report = "";
            
            report += "====================================\n";
            report += "   RAM'S COFFEE SHOP\n";
            report += "   TRANSACTION REPORT\n";
            report += "====================================\n\n";
            
            // Period
            string period = "All Time";
            if (RbToday?.IsChecked == true)
                period = $"Today - {DateTime.Now:MMMM dd, yyyy}";
            else if (RbWeek?.IsChecked == true)
                period = "This Week";
            else if (RbMonth?.IsChecked == true)
                period = $"{DateTime.Now:MMMM yyyy}";
            
            report += $"Period: {period}\n";
            report += $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";
            
            // Summary
            decimal totalRevenue = currentTransactions.Sum(t => t.Amount);
            int transactionCount = currentTransactions.Count;
            decimal avgTransaction = transactionCount > 0 ? totalRevenue / transactionCount : 0;
            decimal totalDiscounts = currentTransactions.Sum(t => t.DiscountAmount + t.LoyaltyRedeemed);
            int totalPoints = currentTransactions.Sum(t => t.PointsEarned);
            
            report += "--- SUMMARY ---\n";
            report += $"Total Transactions: {transactionCount}\n";
            report += $"Total Revenue: ₱{totalRevenue:F2}\n";
            report += $"Average Transaction: ₱{avgTransaction:F2}\n";
            report += $"Total Discounts: ₱{totalDiscounts:F2}\n";
            report += $"Total Points Earned: {totalPoints}\n\n";
            
            // Transaction list
            report += "--- TRANSACTIONS ---\n";
            report += "Date/Time          Order ID            Customer                Amount    Discount  Points\n";
            report += "------------------------------------------------------------------------------------\n";
            
            var sortedTransactions = currentTransactions.OrderByDescending(t => t.DateTime).ToList();
            foreach (var t in sortedTransactions)
            {
                decimal totalDiscount = t.DiscountAmount + t.LoyaltyRedeemed;
                report += $"{t.DateTime:MM/dd HH:mm}         {t.OrderId,-20} {t.CustomerName,-20} " +
                         $"₱{t.Amount,8:F2}  ₱{totalDiscount,7:F2}  +{t.PointsEarned,3}\n";
            }
            
            report += "\n====================================\n";
            report += "         END OF REPORT\n";
            report += "====================================\n";
            
            return report;
        }
        
        
        // HELPER: Show message dialog
        private async void ShowMessage(string title, string message)
        {
            var msgWindow = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            
            var stack = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20
            };
            
            stack.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            
            var btnClose = new Button
            {
                Content = "OK",
                Width = 100,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            btnClose.Click += (s, e) => msgWindow.Close();
            
            stack.Children.Add(btnClose);
            msgWindow.Content = stack;
            
            await msgWindow.ShowDialog(this);
        }
    }
}