using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class InvoiceListViewModel : INotifyPropertyChanged
{
    private readonly InvoiceRepository _invoiceRepo = new();
    private readonly CustomerRepository _customerRepo = new();
    
    private ObservableCollection<Invoice> _invoices = new();
    private ObservableCollection<Customer> _customers = new();
    private Invoice _selectedInvoice = null!;
    private string _startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
    private string _endDate = DateTime.Now.ToString("yyyy-MM-dd");
    private Customer _filterCustomer = null!;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
    
    public event EventHandler<int>? ViewDetailRequested;
    
    public ObservableCollection<Invoice> Invoices
    {
        get => _invoices;
        set { _invoices = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<Customer> Customers
    {
        get => _customers;
        set { _customers = value; OnPropertyChanged(); }
    }
    
    public Invoice SelectedInvoice
    {
        get => _selectedInvoice;
        set 
        { 
            _selectedInvoice = value; 
            OnPropertyChanged();
        }
    }
    

    public string StartDate
    {
        get => _startDate;
        set { _startDate = value; OnPropertyChanged(); }
    }

    public string EndDate
    {
        get => _endDate;
        set { _endDate = value; OnPropertyChanged(); }
    }
    
    public Customer FilterCustomer
    {
        get => _filterCustomer;
        set { _filterCustomer = value; OnPropertyChanged(); }
    }
    
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }
    
    public ICommand LoadInvoicesCommand { get; }
    public ICommand LoadCustomersCommand { get; }
    public ICommand FilterCommand { get; }
    public ICommand ViewDetailCommand { get; }
    public ICommand CancelInvoiceCommand { get; }
    

    
    public InvoiceListViewModel()
    {
        LoadInvoicesCommand = new RelayCommand(LoadInvoices);
        LoadCustomersCommand = new RelayCommand(LoadCustomers);
        FilterCommand = new RelayCommand(ApplyFilter);
        ViewDetailCommand = new RelayCommand(ViewDetail);
        CancelInvoiceCommand = new RelayCommand(CancelInvoice);
        
        LoadCustomers();
        LoadInvoices();
    }
    
    private void LoadInvoices()
    {
        try
        {
            IsLoading = true;
            Invoices = _invoiceRepo.GetAll();
            StatusMessage = $"{Invoices.Count} invoices found";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void LoadCustomers()
    {
        try
        {
            Customers = _customerRepo.GetAll();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading customers: {ex.Message}";
        }
    }
    
    private void ApplyFilter()
    {
        try
        {
            IsLoading = true;
        
            var allInvoices = _invoiceRepo.GetAll();
            var filtered = new ObservableCollection<Invoice>();
        
            // Convertir strings a DateTime
            DateTime start = DateTime.Parse(StartDate);
            DateTime end = DateTime.Parse(EndDate).AddDays(1);
        
            foreach (var invoice in allInvoices)
            {
                bool matchesDate = invoice.IssueDate.Date >= start.Date && invoice.IssueDate.Date <= end.Date;
                bool matchesCustomer = FilterCustomer == null || invoice.CustomerId == FilterCustomer.Id;
            
                if (matchesDate && matchesCustomer)
                {
                    filtered.Add(invoice);
                }
            }
        
            Invoices = filtered;
            StatusMessage = $"{Invoices.Count} invoices match filters";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanViewDetail()
    {
        return SelectedInvoice != null;
    }
    
    private void ViewDetail()
    {
        if (SelectedInvoice != null)
        {
            StatusMessage = $"Viewing invoice {SelectedInvoice.InvoiceNumber}";
            ViewDetailRequested?.Invoke(this, SelectedInvoice.Id);
        }
    }
    
    private bool CanCancelInvoice()
    {
        return SelectedInvoice != null && SelectedInvoice.Status == "ACTIVE";
    }
    
    private void CancelInvoice()
    {
        try
        {
            IsLoading = true;
            _invoiceRepo.Cancel(SelectedInvoice.Id);
            StatusMessage = $"Invoice {SelectedInvoice.InvoiceNumber} cancelled";
            LoadInvoices();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}