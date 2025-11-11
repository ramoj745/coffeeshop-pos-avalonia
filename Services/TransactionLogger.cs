using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoffeeShopPOS.Models;

namespace CoffeeShopPOS.Services
{
    // TransactionLogger handles logging all completed transactions to a text file
    public class TransactionLogger
    {
        private const string FilePath = "transactions.txt";
        
        // method: log a transaction
        // called after a successful payment
        public void LogTransaction(string orderId, string customerId, string customerName,
                                   decimal totalAmount, decimal discountAmount, 
                                   decimal loyaltyRedeemed, int pointsEarned, 
                                   int pointsRedeemed)
        {
            var transaction = new Transaction(
                orderId, 
                customerId, 
                customerName,
                totalAmount, 
                discountAmount, 
                loyaltyRedeemed,
                pointsEarned, 
                pointsRedeemed
            );
            
            try
            {
                // Convert to log string (method from Transaction class)
                string logEntry = transaction.ToLogString();
                
                // Append to file (creates file if it doesnt exist)
                File.AppendAllText(FilePath, logEntry + Environment.NewLine);
                
                Console.WriteLine($"Transaction logged: {orderId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging transaction: {ex.Message}");
            }
        }
        
        
        // method: load all transactions from file
        // returns: list of all transactions (empty list if file doesnt exist)
        public List<Transaction> LoadAllTransactions()
        {
            // check if file exists
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("transactions.txt not found. No transaction history.");
                return new List<Transaction>();
            }
            
            try
            {
                // read all lines from file
                string[] lines = File.ReadAllLines(FilePath);
                
                var transactions = new List<Transaction>();
                
                // parse each line into a Transaction object
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    Transaction? transaction = ParseLogLine(line);
                    
                    if (transaction != null)
                    {
                        transactions.Add(transaction);
                    }
                }
                
                Console.WriteLine($"Loaded {transactions.Count} transactions from file");
                return transactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transactions: {ex.Message}");
                return new List<Transaction>();
            }
        }
        
        
        // method: load transactions for a specific date
        public List<Transaction> LoadTransactionsByDate(DateTime date)
        {
            var allTransactions = LoadAllTransactions();
            
            // filter transactions to only those matching the date
            return allTransactions
                .Where(t => t.DateTime.Date == date.Date)
                .ToList();
        }
        
        
        // method: get total revenue for a date
        public decimal GetTotalRevenueByDate(DateTime date)
        {
            var transactions = LoadTransactionsByDate(date);
            return transactions.Sum(t => t.Amount);
        }
        
        
        // method: get transaction count for a date
        public int GetTransactionCountByDate(DateTime date)
        {
            var transactions = LoadTransactionsByDate(date);
            return transactions.Count;
        }
        
        
        // method: get transactions for a specific customer
        public List<Transaction> LoadTransactionsByCustomer(string customerId)
        {
            var allTransactions = LoadAllTransactions();
            
            return allTransactions
                .Where(t => t.CustomerId == customerId)
                .ToList();
        }
        
        
        // helper method: parse a log line into a Transaction object
        private Transaction? ParseLogLine(string line)
        {
            try
            {
                string[] parts = line.Split('|');
                
                // validate we have all required parts
                if (parts.Length < 9)
                {
                    Console.WriteLine($"Invalid log line (not enough fields): {line}");
                    return null;
                }
                
                // parse each field
                var transaction = new Transaction
                {
                    DateTime = DateTime.Parse(parts[0]),
                    OrderId = parts[1],
                    CustomerId = parts[2],
                    CustomerName = parts[3],
                    Amount = decimal.Parse(parts[4]),
                    DiscountAmount = decimal.Parse(parts[5]),
                    LoyaltyRedeemed = decimal.Parse(parts[6]),
                    PointsEarned = int.Parse(parts[7]),
                    PointsRedeemed = int.Parse(parts[8])
                };
                
                return transaction;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing log line: {ex.Message}");
                Console.WriteLine($"Line: {line}");
                return null;
            }
        }
        
        
        // method: clear all transaction history
        public void ClearAllTransactions()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    Console.WriteLine("✓ Transaction history cleared");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing transactions: {ex.Message}");
            }
        }
    }
}