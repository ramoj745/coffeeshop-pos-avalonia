# Coffee Shop POS System with Loyalty Program

A comprehensive Point-of-Sale system for coffee shops built with C# and Avalonia UI, demonstrating Object-Oriented Programming principles.

##  Features

### Core Functionality
- **Menu Management**: Browse beverages by category (Hot, Iced, Blended)
- **Order Customization**: Choose sizes (Small/Medium/Large) and add-ons
- **Shopping Cart**: Add multiple items, view totals in real-time
- **Customer Loyalty Program**: Earn 1 point per ₱50 spent, redeem 10 points for ₱50 discount
- **Automatic Discounts**: 20% off for Senior Citizens and PWD customers
- **Payment Processing**: Calculate change automatically
- **Receipt Generation**: Detailed receipts with loyalty information

### Business Analytics
- **Transaction History**: View all past transactions
- **Reporting Periods**: Daily, Weekly, Monthly, and All-Time views
- **Summary Metrics**: 
  - Total Revenue
  - Transaction Count
  - Average Transaction Value
  - Total Discounts Given
- **Export Feature**: Generate text reports for external analysis

### Data Persistence
- **Customer Data**: Saved to `customers.json` (includes loyalty points)
- **Transaction Logs**: Appended to `transactions.txt`
- **Cross-Session**: All data persists when app closes

##  OOP Concepts Demonstrated

### 1. Inheritance
- `Beverage` → `HotCoffee`, `IcedCoffee`, `BlendedCoffee`
- `Customer` → `RegularCustomer`, `SeniorCustomer`, `PWDCustomer`

### 2. Encapsulation
- Private fields with public properties
- Business logic hidden in classes
- Repository pattern for data access

### 3. Polymorphism
- `CalculatePrice()` implemented differently per beverage type
- `ApplyDiscount()` behaves differently per customer type

### 4. Abstraction
- Abstract base classes define contracts
- Concrete classes implement specific behavior

##  Technologies Used

- **Language**: C#
- **Framework**: .NET 9.0
- **UI Framework**: Avalonia UI 11.3.8 (cross-platform)
- **Data Format**: JSON (customers), Text (transaction logs)
- **Platform**: macOS, Windows, Linux

##  Project Structure
```
CoffeeShopPOS/
├── Models/                 # Business entities
│   ├── Beverage.cs        # Abstract base class
│   ├── HotCoffee.cs       # Inheritance example
│   ├── IcedCoffee.cs
│   ├── BlendedCoffee.cs
│   ├── Customer.cs        # Abstract base class
│   ├── RegularCustomer.cs # Polymorphism example
│   ├── SeniorCustomer.cs
│   ├── PWDCustomer.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── AddOn.cs
│   ├── LoyaltyAccount.cs
│   └── Transaction.cs
├── Services/              # Data access layer
│   ├── CustomerRepository.cs
│   └── TransactionLogger.cs
├── Views/                 # User interface
│   ├── MainWindow.axaml
│   ├── OrderWindow.axaml
│   ├── CheckoutWindow.axaml
│   └── TransactionHistoryWindow.axaml                  
├── customers.json # Generated at runtime
├── transactions.txt # Generated at runtime
```

##  Getting Started

### Prerequisites
- .NET 9.0 SDK
- Any IDE (VS Code, Visual Studio, Rider)

### Installation
```bash
# Clone the repository
git clone https://github.com/ramoj745/coffeeshop-pos-avalonia.git

# Navigate to project directory
cd CoffeeShopPOS

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

### First Run
The application will automatically:
- Creates 3 test customers (C00001, C00002, C00003)
- Seeds sample transactions across different dates
- Generate `customers.json` and `transactions.txt`

##  Usage Guide

### Starting an Order
1. Click **"Walk-In Order"** for walk-in customer
2. Or click **"Customer Order"** and enter ID (e.g., C00001)

### Building the Order
1. Click on a beverage from the menu
2. Select size and add-ons
3. Set quantity
4. Click **"Add to Order"**
5. Repeat for multiple items

### Checkout
1. Click **"Proceed to Checkout"**
2. Select customer type (Regular/Senior/PWD) for discounts
3. Redeem loyalty points if available
4. Enter payment amount
5. Click **"Process Payment"**
6. View receipt

### Viewing Reports
1. Click **"Transaction History"**
2. Select period (Today/Week/Month/All)
3. View summary metrics and transaction list
4. Click **"Export to File"** for text report

##  Business Rules

### Pricing
- **Sizes**: Small (base), Medium (+₱20), Large (+₱40) for hot (+₱5 or +₱10 to medium/large for iced or blended)
- **Add-ons**: Extra Shot (₱25), Whipped Cream (₱15), Caramel Drizzle (₱20)

### Loyalty Program
- **Earn**: 1 point per ₱50 spent (rounded down)
- **Redeem**: Must be multiples of 10 points
- **Value**: 10 points = ₱50 discount

### Discounts
- **Senior Citizens**: 20% off subtotal
- **PWD**: 20% off subtotal
- **Limitation**: Only one discount type per transaction

##  Sample Test Data

### Test Customers
| ID | Name | Type | Points |
|----|------|------|--------|
| C00001 | Juan Dela Cruz | Regular | 15 |
| C00002 | Maria Santos | Senior | 42 |
| C00003 | Pedro Reyes | PWD | 8 |

### Sample Menu Items
| Code | Name | Category | Base Price |
|------|------|----------|------------|
| C001 | Americano | Hot | ₱95 |
| C002 | Cappuccino | Hot | ₱120 |
| C003 | Caramel Macchiato | Iced | ₱145 |
| C004 | Matcha Latte | Iced | ₱130 |
| C005 | Mocha Frappuccino | Blended | ₱165 |

##  Developer

**Ramoj Mabilangan** -
- GitHub: [@ramoj745](https://github.com/ramoj745)

##  License

This project is for educational purposes as part of an Object-Oriented Programming course.

---

**Note**: This is a student project demonstrating OOP concepts. While functional, it's designed for educational purposes rather than production deployment.
