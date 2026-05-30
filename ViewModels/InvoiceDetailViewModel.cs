using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class InvoiceDetailViewModel : INotifyPropertyChanged
{
    private readonly InvoiceRepository _invoiceRepo = new();
    private readonly InvoiceDetailRepository _detailRepo = new();
    private readonly CustomerRepository _customerRepo = new();
    private readonly ProductRepository _productRepo = new();
    
    private Invoice _currentInvoice = null!;
    private ObservableCollection<InvoiceDetail> _details = new();
    private Customer _customer = null!;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
    
    public Invoice CurrentInvoice
    {
        get => _currentInvoice;
        set { _currentInvoice = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<InvoiceDetail> Details
    {
        get => _details;
        set { _details = value; OnPropertyChanged(); }
    }
    
    public Customer Customer
    {
        get => _customer;
        set { _customer = value; OnPropertyChanged(); }
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
    
    public string InvoiceNumber => CurrentInvoice?.InvoiceNumber ?? string.Empty;
    public string IssueDate => CurrentInvoice?.IssueDate.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
    public string SubtotalFormatted => $"S/ {CurrentInvoice?.Subtotal:F2}";
    public string TaxFormatted => $"S/ {CurrentInvoice?.Tax:F2}";
    public string TotalFormatted => $"S/ {CurrentInvoice?.Total:F2}";
    public string Status => CurrentInvoice?.Status ?? string.Empty;
    
    public ICommand LoadInvoiceCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand BackCommand { get; }
    
    public InvoiceDetailViewModel()
    {
        LoadInvoiceCommand = new RelayCommand(LoadInvoice);
        PrintCommand = new RelayCommand(PrintInvoice, CanPrint);
        BackCommand = new RelayCommand(GoBack);
    }
    
    public void SetInvoiceId(int invoiceId)
    {
        try
        {
            IsLoading = true;
            CurrentInvoice = _invoiceRepo.GetById(invoiceId);
            if (CurrentInvoice != null)
            {
                Customer = _customerRepo.GetById(CurrentInvoice.CustomerId);
                LoadDetails();
                UpdateDisplayProperties();
                StatusMessage = $"Invoice {CurrentInvoice.InvoiceNumber} loaded";
            }
            else
            {
                StatusMessage = "Invoice not found";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading invoice: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void LoadInvoice()
    {
        // This method is called when navigating directly
        // Actual implementation depends on navigation pattern
    }
    
    private void LoadDetails()
    {
        if (CurrentInvoice != null)
        {
            Details = _detailRepo.GetByInvoiceId(CurrentInvoice.Id);
            
            // Load product names for each detail
            foreach (var detail in Details)
            {
                var product = _productRepo.GetById(detail.ProductId);
                if (product != null)
                {
                    detail.Product = product;
                }
            }
        }
    }
    
    private void UpdateDisplayProperties()
    {
        OnPropertyChanged(nameof(InvoiceNumber));
        OnPropertyChanged(nameof(IssueDate));
        OnPropertyChanged(nameof(SubtotalFormatted));
        OnPropertyChanged(nameof(TaxFormatted));
        OnPropertyChanged(nameof(TotalFormatted));
        OnPropertyChanged(nameof(Status));
    }
    
    private bool CanPrint()
    {
        return CurrentInvoice != null && CurrentInvoice.Status == "ACTIVE";
    }
    
    private void PrintInvoice()
    {
        // This will be implemented later with PDF generation
        StatusMessage = $"Printing invoice {CurrentInvoice?.InvoiceNumber}";
        // TODO: Implement PDF generation and print
    }
    
    private void GoBack()
    {
        // Navigation back to invoice list
        // This will be implemented when views are created
        StatusMessage = "Returning to invoice list";
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}