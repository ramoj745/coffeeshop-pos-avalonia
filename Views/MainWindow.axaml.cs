using System;
using Avalonia.Controls;
using CoffeeShopPOS.Models;

namespace CoffeeShopPOS.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        TestBeverageClass();
    }

    private void TestBeverageClass()
    {

        HotCoffee hotAmericano = new()
        {
            Code = "C002",
            Name = "Hot Americano",
            BasePrice = 65
        };

        IcedCoffee icedAmericano = new()
        {
            Code = "C001",
            Name = "Iced Americano",
            BasePrice = 75
        };

        // tests
        PrintBeveragePrices(hotAmericano);
        PrintBeveragePrices(icedAmericano);

    }

    private void PrintBeveragePrices(Beverage beverage)
    {
        Console.WriteLine("Type of Beverage: " + beverage.Category);
        Console.WriteLine("Name of Beverage: " + beverage.Name);
        Console.WriteLine("Small: P" + beverage.CalculatePrice("S"));
        Console.WriteLine("Medium: P" + beverage.CalculatePrice("M"));
        Console.WriteLine("Large: P" + beverage.CalculatePrice("L"));

    }
    
}