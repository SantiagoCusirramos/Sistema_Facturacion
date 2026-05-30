using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
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
            case "Customers":
                // CurrentView = new CustomerViewModel();
                break;
            case "Products":
                // CurrentView = new ProductViewModel();
                break;
            case "Categories":
                // CurrentView = new CategoryViewModel();
                break;
            case "Sales":
                // CurrentView = new SaleViewModel();
                break;
            case "Invoices":
                // CurrentView = new InvoiceListViewModel();
                break;
            case "Kardex":
                // CurrentView = new KardexViewModel();
                break;
            case "Reports":
                // CurrentView = new ReportViewModel();
                break;
            case "Dashboard":
            default:
                CurrentModule = "Dashboard";
                // CurrentView = new DashboardViewModel();
                break;
        }
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