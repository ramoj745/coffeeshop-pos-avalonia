using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CoffeeShopPOS.Models;

namespace CoffeeShopPOS.Services
{
    // handles all file i/o operations for customers
    public class CustomerRepository
    {
        // the file path where customers are stored
        private const string FilePath = "customers.json";

        // method: load all customers from the json file
        public List<Customer> LoadAllCustomers()
        {
            // check if the file exists first
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("customers.json not found. Starting with an empty list.");
                return new List<Customer>();
            }

            try
            {
                string json = File.ReadAllText(FilePath);

                var customerDataList = JsonSerializer.Deserialize<List<CustomerData>>(json);

                if (customerDataList == null)
                {
                    Console.WriteLine("Failed to deserialize customers.json");
                    return new List<Customer>();
                }

                // convert customerData objects to actual Customer objects
                var customers = new List<Customer>();
                foreach (var data in customerDataList)
                {
                    Customer customer = CreateCustomerFromData(data);
                    customers.Add(customer);
                }

                Console.WriteLine($"Loaded {customers.Count} customers");
                return customers;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Loading customers: {ex.Message}");
                return new List<Customer>();
            }
        }

        // method: load a specific customer by ID
        public Customer? LoadCustomer(string customerId)
        {
            var customers = LoadAllCustomers();
            return customers.FirstOrDefault(c => c.Id == customerId);
        }

        // method: save a customer (add new or update existing)
        public void SaveCustomer(Customer customer)
        {
            var customers = LoadAllCustomers();

            // remove customers with duplicate IDs
            customers.RemoveAll(c => c.Id == customer.Id);

            customers.Add(customer);

            // convert all customers to CustomerData for serialization
            var customerDataList = new List<CustomerData>();
            foreach (var c in customers)
            {
                customerDataList.Add(CreateDataFromCustomer(c));
            }

            try
            {
                // serialize JSON with formatting
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(customerDataList, options);

                // write to file
                File.WriteAllText(FilePath, json);
                Console.WriteLine("Saved customer");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving customer {ex.Message}");
            }
        }

        // method: generate customer ID
        public string GenerateNewCustomerId()
        {
            var customers = LoadAllCustomers();

            if (customers.Count == 0)
            {
                return "C00001";
            }

            int maxId = 0;
            foreach (var customer in customers)
            {
                string numberPart = customer.Id.Replace("C", "");
                if (int.TryParse(numberPart, out int idNumber))
                {
                    if (idNumber > maxId)
                        maxId = idNumber;
                }
            }

            // generate next id
            int newId = maxId + 1;
            return $"C{newId:D5}";
        }

        // helper method: convert CustomerData to proper Customer object
        private Customer CreateCustomerFromData(CustomerData data)
        {
            Customer customer;

            switch (data.CustomerType)
            {
                case "Senior":
                    customer = new SeniorCustomer(data.CustomerId, data.Name);
                    break;
                case "PWD":
                    customer = new PWDCustomer(data.CustomerId, data.Name);
                    break;
                default: // regular
                    customer = new RegularCustomer(data.CustomerId, data.Name);
                    break;
            }

            customer.DateRegistered = data.DateRegistered;

            // create loyalt account with saved points
            customer.LoyaltyAccount = new LoyaltyAccount(data.CustomerId, data.LoyaltyPoints);
            return customer;
        }

        // helper method: convert Customer object to CustomerData for JSON
        private CustomerData CreateDataFromCustomer(Customer customer)
        {
            return new CustomerData
            {
                CustomerId = customer.Id,
                Name = customer.Name,
                CustomerType = customer.Type,
                LoyaltyPoints = customer.LoyaltyAccount?.Points ?? 0,
                DateRegistered = customer.DateRegistered
            };
        }
    }

    // simple data structure for JSON serialization, used only for file storage
    // helper class: we need this because Customer is abstract
    // and has complex inheritance
    public class CustomerData
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }
        public DateTime DateRegistered { get; set; }

    }

}