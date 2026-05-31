using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class ReportViewModel : INotifyPropertyChanged
{
    private readonly InvoiceRepository _invoiceRepo = new();
    private readonly ProductRepository _productRepo = new();
    private readonly CustomerRepository _customerRepo = new();
    
    private DateTime _startDate = DateTime.Now.AddDays(-30);
    private DateTime _endDate = DateTime.Now;
    private ObservableCollection<SalesReportItem> _salesReport = new();
    private decimal _totalSales = 0;
    private int _totalInvoices = 0;
    
    private ObservableCollection<ProductReportItem> _productReport = new();
    private string _searchProductName = string.Empty;
    
    private bool _isLoading = false;
    private string _statusMessage = string.Empty;
    private int _selectedTabIndex = 0;
    
    private string _startDateString = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
    private string _endDateString = DateTime.Now.ToString("yyyy-MM-dd");

    public string StartDateString
    {
        get => _startDateString;
        set 
        { 
            _startDateString = value; 
            OnPropertyChanged();
            if (DateTime.TryParse(value, out DateTime parsed))
            {
                StartDate = parsed;
            }
        }
    }

    public string EndDateString
    {
        get => _endDateString;
        set 
        { 
            _endDateString = value; 
            OnPropertyChanged();
            if (DateTime.TryParse(value, out DateTime parsed))
            {
                EndDate = parsed;
            }
        }
    }
    
    public DateTime StartDate
    {
        get => _startDate;
        set { _startDate = value; OnPropertyChanged(); }
    }
    
    public DateTime EndDate
    {
        get => _endDate;
        set { _endDate = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<SalesReportItem> SalesReport
    {
        get => _salesReport;
        set { _salesReport = value; OnPropertyChanged(); }
    }
    
    public decimal TotalSales
    {
        get => _totalSales;
        set { _totalSales = value; OnPropertyChanged(); }
    }
    
    public int TotalInvoices
    {
        get => _totalInvoices;
        set { _totalInvoices = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<ProductReportItem> ProductReport
    {
        get => _productReport;
        set { _productReport = value; OnPropertyChanged(); }
    }
    
    public string SearchProductName
    {
        get => _searchProductName;
        set { _searchProductName = value; OnPropertyChanged(); }
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }
    
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }
    
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set { _selectedTabIndex = value; OnPropertyChanged(); }
    }
    
    public ICommand GenerateSalesReportCommand { get; }
    public ICommand GenerateProductReportCommand { get; }
    public ICommand SearchProductReportCommand { get; }
    public ICommand ExportSalesReportCommand { get; }
    public ICommand ExportProductReportCommand { get; }
    
    public ReportViewModel()
    {
        GenerateSalesReportCommand = new RelayCommand(GenerateSalesReport);
        GenerateProductReportCommand = new RelayCommand(GenerateProductReport);
        SearchProductReportCommand = new RelayCommand(SearchProductReport);
        ExportSalesReportCommand = new RelayCommand(ExportSalesReport, CanExport);
        ExportProductReportCommand = new RelayCommand(ExportProductReport, CanExport);
        
        GenerateSalesReport();
        GenerateProductReport();
    }
    
    private void GenerateSalesReport()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating sales report...";
            
            var allInvoices = _invoiceRepo.GetAll();
            var filtered = new ObservableCollection<SalesReportItem>();
            decimal total = 0;
            
            DateTime start = StartDate.Date;
            DateTime end = EndDate.Date.AddDays(1);
            
            foreach (var invoice in allInvoices)
            {
                if (invoice.IssueDate >= start && invoice.IssueDate <= end && invoice.Status == "ACTIVE")
                {
                    filtered.Add(new SalesReportItem
                    {
                        InvoiceNumber = invoice.InvoiceNumber,
                        Date = invoice.IssueDate,
                        CustomerName = invoice.Customer?.Name ?? "Unknown",
                        Subtotal = invoice.Subtotal,
                        Tax = invoice.Tax,
                        Total = invoice.Total
                    });
                    total += invoice.Total;
                }
            }
            
            SalesReport = filtered;
            TotalSales = total;
            TotalInvoices = filtered.Count;
            StatusMessage = $"Sales report generated: {TotalInvoices} invoices, Total: S/ {TotalSales:F2}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error generating sales report: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void GenerateProductReport()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating product report...";
            
            var products = _productRepo.GetAll();
            var report = new ObservableCollection<ProductReportItem>();
            
            foreach (var product in products)
            {
                report.Add(new ProductReportItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Stock = product.Stock,
                    Price = product.Price,
                    TotalValue = product.Stock * product.Price,
                    CategoryName = product.Category?.Name ?? "Uncategorized"
                });
            }
            
            ProductReport = report;
            StatusMessage = $"Product report generated: {ProductReport.Count} products";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error generating product report: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void SearchProductReport()
    {
        if (string.IsNullOrWhiteSpace(SearchProductName))
        {
            GenerateProductReport();
            return;
        }
        
        var filtered = new ObservableCollection<ProductReportItem>();
        foreach (var item in ProductReport)
        {
            if (item.ProductName.Contains(SearchProductName, StringComparison.OrdinalIgnoreCase))
            {
                filtered.Add(item);
            }
        }
        
        ProductReport = filtered;
        StatusMessage = $"Found {ProductReport.Count} products matching '{SearchProductName}'";
    }
    
    private bool CanExport()
    {
        return SalesReport.Count > 0 || ProductReport.Count > 0;
    }
    
    private void ExportSalesReport()
    {
        StatusMessage = "Exporting sales report to CSV...";
        // TODO: Implement CSV/PDF export
    }
    
    private void ExportProductReport()
    {
        StatusMessage = "Exporting product report to CSV...";
        // TODO: Implement CSV/PDF export
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public class SalesReportItem
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    
    public string DateFormatted => Date.ToString("dd/MM/yyyy");
    public string SubtotalFormatted => $"S/ {Subtotal:F2}";
    public string TaxFormatted => $"S/ {Tax:F2}";
    public string TotalFormatted => $"S/ {Total:F2}";
}

public class ProductReportItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public decimal TotalValue { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    
    public string PriceFormatted => $"S/ {Price:F2}";
    public string TotalValueFormatted => $"S/ {TotalValue:F2}";
}