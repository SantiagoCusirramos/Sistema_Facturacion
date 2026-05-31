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
        set 
        { 
            _currentInvoice = value; 
            OnPropertyChanged();
            OnPropertyChanged(nameof(InvoiceNumber));
            OnPropertyChanged(nameof(IssueDate));
            OnPropertyChanged(nameof(SubtotalFormatted));
            OnPropertyChanged(nameof(TaxFormatted));
            OnPropertyChanged(nameof(TotalFormatted));
            OnPropertyChanged(nameof(Status));
        }
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
    
    public string InvoiceNumber => CurrentInvoice?.InvoiceNumber ?? "N/A";
    public string IssueDate => CurrentInvoice?.IssueDate.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
    public string SubtotalFormatted => CurrentInvoice != null ? $"S/ {CurrentInvoice.Subtotal:F2}" : "S/ 0.00";
    public string TaxFormatted => CurrentInvoice != null ? $"S/ {CurrentInvoice.Tax:F2}" : "S/ 0.00";
    public string TotalFormatted => CurrentInvoice != null ? $"S/ {CurrentInvoice.Total:F2}" : "S/ 0.00";
    public string Status => CurrentInvoice?.Status ?? "N/A";
    public string StatusColor => CurrentInvoice?.Status == "ACTIVE" ? "#27AE60" : "#E74C3C";
    
    public ICommand PrintCommand { get; }
    public ICommand BackCommand { get; }
    
    public event EventHandler? BackRequested;
    
    public InvoiceDetailViewModel()
    {
        PrintCommand = new RelayCommand(PrintInvoice, CanPrint);
        BackCommand = new RelayCommand(GoBack);
    }
    
    public void SetInvoiceId(int invoiceId)
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading invoice...";
            
            CurrentInvoice = _invoiceRepo.GetById(invoiceId);
            
            if (CurrentInvoice != null)
            {
                Customer = _customerRepo.GetById(CurrentInvoice.CustomerId);
                
                LoadDetails();
                
                StatusMessage = $"✅ Invoice {CurrentInvoice.InvoiceNumber} loaded successfully";
                Console.WriteLine($"Invoice loaded: {CurrentInvoice.InvoiceNumber}, Details: {Details.Count}");
            }
            else
            {
                StatusMessage = "❌ Invoice not found";
                Console.WriteLine($"Invoice {invoiceId} not found");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error loading invoice: {ex.Message}";
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void LoadDetails()
    {
        try
        {
            if (CurrentInvoice == null) return;
            
            Details = _detailRepo.GetByInvoiceId(CurrentInvoice.Id);
        
            foreach (var detail in Details)
            {
                if (detail.Product == null && detail.ProductId > 0)
                {
                    var product = _productRepo.GetById(detail.ProductId);
                    if (product != null)
                    {
                        detail.Product = product;
                    }
                }
            
                Console.WriteLine($"Detail: ProductId={detail.ProductId}, ProductName={detail.Product?.Name}, Qty={detail.Quantity}");
            }
        
            StatusMessage = $"Loaded {Details.Count} products";
            Console.WriteLine($"Total details loaded: {Details.Count}");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading details: {ex.Message}";
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    
    private bool CanPrint()
    {
        return CurrentInvoice != null && CurrentInvoice.Status == "ACTIVE";
    }
    
    private void PrintInvoice()
    {
        StatusMessage = $"🖨️ Printing invoice {CurrentInvoice?.InvoiceNumber}";
        // TODO: Implement PDF generation
    }
    
    private void GoBack()
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}