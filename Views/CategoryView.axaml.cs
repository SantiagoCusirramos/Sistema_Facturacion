using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;
using System;

namespace Sistema_Facturacion.Views;

public partial class CategoryView : UserControl
{
    public CategoryView()
    {
        InitializeComponent();
        var vm = new CategoryViewModel();
        DataContext = vm;
        
        // Debug
        Console.WriteLine($"CategoryView created - Categories count: {vm.Categories.Count}");
    }
}