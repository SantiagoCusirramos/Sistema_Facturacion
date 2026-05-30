using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private object _currentView = null!;
    private string _windowTitle = "Sistema de Facturación";
    private string _currentModule = "Inicio";
    
    
    
    public object CurrentView
    {
        get => _currentView;
        set { _currentView = value; OnPropertyChanged(); }
    }
    
    public string WindowTitle
    {
        get => _windowTitle;
        set { _windowTitle = value; OnPropertyChanged(); }
    }
    
    public string CurrentModule
    {
        get => _currentModule;
        set { _currentModule = value; OnPropertyChanged(); }
    }
    
    public ICommand ShowCustomersCommand { get; }
    public ICommand ShowProductsCommand { get; }
    public ICommand ShowCategoriesCommand { get; }
    public ICommand ShowSalesCommand { get; }
    public ICommand ShowInvoicesCommand { get; }
    public ICommand ShowKardexCommand { get; }
    public ICommand ShowReportsCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand ExitCommand { get; }
    
    public MainWindowViewModel()
    {
        ShowCustomersCommand = new RelayCommand(() => ShowView("Customers"));
        ShowProductsCommand = new RelayCommand(() => ShowView("Products"));
        ShowCategoriesCommand = new RelayCommand(() => ShowView("Categories"));
        ShowSalesCommand = new RelayCommand(() => ShowView("Sales"));
        ShowInvoicesCommand = new RelayCommand(() => ShowView("Invoices"));
        ShowKardexCommand = new RelayCommand(() => ShowView("Kardex"));
        ShowReportsCommand = new RelayCommand(() => ShowView("Reports"));
        LogoutCommand = new RelayCommand(Logout);
        ExitCommand = new RelayCommand(Exit);
        
        ShowView("Dashboard");
    }
    
    private void ShowView(string viewName)
{
    CurrentModule = viewName;
    
    switch (viewName)
    {
        case "Dashboard":
            CurrentView = CreateDashboardView();
            break;
        case "Categories":
            CurrentView = new Views.CategoryView();
            break;
        case "Customers":
            CurrentView = new Views.CustomerView();
            break;
        case "Products":
            CurrentView = new Views.ProductView();
            break;
        case "Sales":
            CurrentView = new Views.SaleView();
            break;
        case "Invoices":
            var invoiceListView = new Views.InvoiceListView();
            var invoiceListVM = new InvoiceListViewModel();
            invoiceListView.DataContext = invoiceListVM;
            invoiceListVM.ViewDetailRequested += OnViewDetailRequested;
            CurrentView = invoiceListView;
            break;
        default:
            CurrentView = CreateDashboardView();
            break;
    }
}
    
    private void OnViewDetailRequested(object? sender, int invoiceId)
    {
        var detailView = new Views.InvoiceDetailView();
        var detailVM = new InvoiceDetailViewModel();
        detailView.DataContext = detailVM;
        detailView.LoadInvoice(invoiceId);
    
        // Suscribirse al evento Back para regresar a la lista
        detailVM.BackRequested += (s, e) =>
        {
            // Regresar a la lista de facturas
            var invoiceListView = new Views.InvoiceListView();
            var invoiceListVM = new InvoiceListViewModel();
            invoiceListView.DataContext = invoiceListVM;
            invoiceListVM.ViewDetailRequested += OnViewDetailRequested;
            CurrentView = invoiceListView;
        };
    
        CurrentView = detailView;
    }

    private object CreateDashboardView()
    {
        var stackPanel = new StackPanel();
        
        // Título principal
        stackPanel.Children.Add(new TextBlock 
        { 
            Text = "🏠 INVOICE SYSTEM DASHBOARD", 
            FontSize = 28,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            Foreground = Avalonia.Media.Brushes.White,
            Margin = new Avalonia.Thickness(0, 30, 0, 20),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });
        
        // Subtítulo
        stackPanel.Children.Add(new TextBlock 
        { 
            Text = "Welcome to the Professional Invoicing System",
            FontSize = 16,
            Foreground = Avalonia.Media.Brushes.LightGray,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 0, 0, 40)
        });
        
        // Panel de estadísticas (simulado)
        var statsGrid = new Grid();
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        statsGrid.Margin = new Avalonia.Thickness(20);
        
        statsGrid.Children.Add(CreateStatCard("📊", "Today's Sales", "S/ 0.00", 0));
        statsGrid.Children.Add(CreateStatCard("👥", "Customers", "0", 1));
        statsGrid.Children.Add(CreateStatCard("📦", "Products", "0", 2));
        
        stackPanel.Children.Add(statsGrid);
        
        // Mensaje de bienvenida
        stackPanel.Children.Add(new TextBlock 
        { 
            Text = "Select an option from the menu to start working",
            FontSize = 14,
            Foreground = Avalonia.Media.Brushes.Gray,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 40, 0, 0)
        });
        
        return stackPanel;
    }

    private Border CreateStatCard(string icon, string title, string value, int column)
    {
        var card = new Border();
        card.Background = Avalonia.Media.Brushes.DarkSlateBlue;
        card.CornerRadius = new Avalonia.CornerRadius(10);
        card.Padding = new Avalonia.Thickness(20);
        card.Margin = new Avalonia.Thickness(10);
        Grid.SetColumn(card, column);
        
        var stack = new StackPanel();
        stack.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        
        stack.Children.Add(new TextBlock 
        { 
            Text = icon, 
            FontSize = 40,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });
        
        stack.Children.Add(new TextBlock 
        { 
            Text = title, 
            FontSize = 14,
            Foreground = Avalonia.Media.Brushes.LightGray,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 10, 0, 5)
        });
        
        stack.Children.Add(new TextBlock 
        { 
            Text = value, 
            FontSize = 24,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            Foreground = Avalonia.Media.Brushes.White,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });
        
        card.Child = stack;
        return card;
    }
    
    private void Logout()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                window.Close();
            }
            
            // var loginWindow = new LoginWindow();
            // loginWindow.Show();

        }
        
        System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "");
        Environment.Exit(0);
    }
    
    private void Exit()
    {
        Environment.Exit(0);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}