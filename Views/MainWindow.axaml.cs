using System;
using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        // Debug
        Console.WriteLine("MainWindow created");
    }
}