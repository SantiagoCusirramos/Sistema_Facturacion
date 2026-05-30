using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class SaleViewModel : INotifyPropertyChanged
{
    private readonly CustomerRepository _customerRepo = new();
    private readonly ProductRepository _productRepo = new();
    private readonly InvoiceRepository _invoiceRepo = new();
    private readonly InvoiceDetailRepository _detailRepo = new();
    private readonly KardexRepository _kardexRepo = new();
    
    private ObservableCollection<Customer> _customers = new();
    private ObservableCollection<Product> _products = new();
    private ObservableCollection<InvoiceDetail> _cart = new();
    
    private Customer _selectedCustomer = null!;
    private Product _selectedProduct = null!;
    private string _searchProductText = string.Empty;
    private int _quantity = 1;
    private decimal _subtotal = 0;
    private decimal _tax = 0;
    private decimal _total = 0;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
    
    private InvoiceDetail _selectedCartItem = null!;
    
    public ObservableCollection<Customer> Customers
    {
        get => _customers;
        set { _customers = value; OnPropertyChanged(); }
    }
    public ObservableCollection<Product> Products
    {
        get => _products;
        set { _products = value; OnPropertyChanged(); }
    }
    public ObservableCollection<InvoiceDetail> Cart
    {
        get => _cart;
        set { _cart = value; OnPropertyChanged(); }
    }
    public Customer SelectedCustomer
    {
        get => _selectedCustomer;
        set { _selectedCustomer = value; OnPropertyChanged(); }
    }
    public Product SelectedProduct
    {
        get => _selectedProduct;
        set 
        { 
            _selectedProduct = value; 
            OnPropertyChanged();
            if (value != null)
            {
                Quantity = 1;
            }
        }
    }
    
    public InvoiceDetail SelectedCartItem
    {
        get => _selectedCartItem;
        set 
        { 
            _selectedCartItem = value; 
            OnPropertyChanged();
        }
    }
    
    public string SearchProductText
    {
        get => _searchProductText;
        set { _searchProductText = value; OnPropertyChanged(); }
    }
    
    public int Quantity
    {
        get => _quantity;
        set 
        { 
            _quantity = value; 
            OnPropertyChanged();
            CalculateTotals();
        }
    }
    
    public decimal Subtotal
    {
        get => _subtotal;
        set { _subtotal = value; OnPropertyChanged(); }
    }
    
    public decimal Tax
    {
        get => _tax;
        set { _tax = value; OnPropertyChanged(); }
    }
    
    public decimal Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(); }
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
    
    public ICommand LoadDataCommand { get; }
    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand ProcessSaleCommand { get; }
    public ICommand SearchProductCommand { get; }
    public ICommand ClearSaleCommand { get; }
    
    public SaleViewModel()
    {
        LoadDataCommand = new RelayCommand(LoadData);
        AddToCartCommand = new RelayCommand(AddToCart, CanAddToCart);
        RemoveFromCartCommand = new RelayCommand(RemoveFromCart, CanRemoveFromCart);
        ProcessSaleCommand = new RelayCommand(ProcessSale, CanProcessSale);
        SearchProductCommand = new RelayCommand(SearchProducts);
        ClearSaleCommand = new RelayCommand(ClearSale);
        
        LoadData();
    }
    
    private void LoadData()
    {
        try
        {
            IsLoading = true;
            Customers = _customerRepo.GetAll();
            Products = _productRepo.GetAll();
            StatusMessage = $"Ready - {Products.Count} products available";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void SearchProducts()
    {
        if (string.IsNullOrWhiteSpace(SearchProductText))
        {
            Products = _productRepo.GetAll();
        }
        else
        {
            var filtered = new ObservableCollection<Product>();
            foreach (var product in _productRepo.GetAll())
            {
                if (product.Name.Contains(SearchProductText, StringComparison.OrdinalIgnoreCase))
                {
                    filtered.Add(product);
                }
            }
            Products = filtered;
        }
        StatusMessage = $"{Products.Count} products found";
    }
    
    private bool CanAddToCart()
    {
        return SelectedProduct != null && Quantity > 0 && Quantity <= SelectedProduct.Stock;
    }
    
    private void AddToCart()
    {
        var existingItem = GetCartItem(SelectedProduct.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity += Quantity;
            existingItem.Subtotal = existingItem.Quantity * existingItem.UnitPrice;
        }
        else
        {
            Cart.Add(new InvoiceDetail
            {
                ProductId = SelectedProduct.Id,
                Product = SelectedProduct,
                Quantity = Quantity,
                UnitPrice = SelectedProduct.Price,
                Subtotal = Quantity * SelectedProduct.Price
            });
        }
        
        CalculateTotals();
        StatusMessage = $"{Quantity} x {SelectedProduct.Name} added";
        SelectedProduct = null!;
        Quantity = 1;
    }
    
    private InvoiceDetail? GetCartItem(int productId)
    {
        foreach (var item in Cart)
        {
            if (item.ProductId == productId)
                return item;
        }
        return null;
    }
    
    private bool CanRemoveFromCart()
    {
        return Cart.Count > 0;
    }
    
    private void RemoveFromCart()
    {
        if (SelectedCartItem != null)
        {
            Cart.Remove(SelectedCartItem);
            CalculateTotals();
            StatusMessage = "Item removed from cart";
            SelectedCartItem = null!;
        }
    }
    
    private void CalculateTotals()
    {
        Subtotal = 0;
        foreach (var item in Cart)
        {
            Subtotal += item.Subtotal;
        }
        Tax = Subtotal * 0.18m; // 18% tax
        Total = Subtotal + Tax;
    }
    
    private bool CanProcessSale()
    {
        return SelectedCustomer != null && Cart.Count > 0;
    }
    
    private void ProcessSale()
    {
        try
        {
            IsLoading = true;
            
            string invoiceNumber = _invoiceRepo.GenerateInvoiceNumber();
            
            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                CustomerId = SelectedCustomer.Id,
                PaymentMethodId = 1, // Cash by default
                DocumentTypeId = 1,  // Invoice by default
                IssueDate = DateTime.Now,
                Subtotal = Subtotal,
                Tax = Tax,
                Total = Total,
                Status = "ACTIVE"
            };
            
            int invoiceId = _invoiceRepo.Create(invoice);
            
            foreach (var item in Cart)
            {
                item.InvoiceId = invoiceId;
                _detailRepo.Create(item);
                
                var product = _productRepo.GetById(item.ProductId);
                if (product != null)
                {
                    int newStock = product.Stock - item.Quantity;
                    _productRepo.UpdateStock(item.ProductId, newStock);
                    
                    // Register in Kardex
                    _kardexRepo.RegisterSale(item.ProductId, item.Quantity, item.Id);
                }
            }
            
            StatusMessage = $"Invoice {invoiceNumber} created successfully";
            ClearSale();
            LoadData();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error processing sale: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void ClearSale()
    {
        Cart.Clear();
        SelectedCustomer = null!;
        SelectedProduct = null!;
        Quantity = 1;
        Subtotal = 0;
        Tax = 0;
        Total = 0;
        SearchProductText = string.Empty;
        Products = _productRepo.GetAll();
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}