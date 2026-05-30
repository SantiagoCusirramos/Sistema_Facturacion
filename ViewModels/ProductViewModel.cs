using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class ProductViewModel : INotifyPropertyChanged
{
    private readonly ProductRepository _productRepo = new();
    private readonly CategoryRepository _categoryRepo = new();
    
    private ObservableCollection<Product> _products = new();
    private ObservableCollection<Category> _categories = new();
    private Product _selectedProduct = null!;
    private string _searchText = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
    
    // Form properties
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _brand = string.Empty;
    private decimal _price = 0;
    private int _stock = 0;
    private int _minStock = 0;
    private int? _categoryId = null;
    
    private Category _selectedCategory = null!;
    
    public ObservableCollection<Product> Products
    {
        get => _products;
        set { _products = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set { _categories = value; OnPropertyChanged(); }
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
                Name = value.Name;
                Description = value.Description ?? string.Empty;
                Price = value.Price;
                Stock = value.Stock;
                CategoryId = value.CategoryId;
            }
        }
    }
    
    public Category SelectedCategory
    {
        get => _selectedCategory;
        set 
        { 
            _selectedCategory = value; 
            OnPropertyChanged();
            if (value != null)
            {
                CategoryId = value.Id;
            }
        }
    }
    
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
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
    
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }
    
    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }
    
    public string Brand
    {
        get => _brand;
        set { _brand = value; OnPropertyChanged(); }
    }
    
    public decimal Price
    {
        get => _price;
        set { _price = value; OnPropertyChanged(); }
    }
    
    public int Stock
    {
        get => _stock;
        set { _stock = value; OnPropertyChanged(); }
    }
    
    public int MinStock
    {
        get => _minStock;
        set { _minStock = value; OnPropertyChanged(); }
    }
    
    public int? CategoryId
    {
        get => _categoryId;
        set { _categoryId = value; OnPropertyChanged(); }
    }
    
    // Commands
    public ICommand LoadProductsCommand { get; }
    public ICommand LoadCategoriesCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFormCommand { get; }
    public ICommand SearchCommand { get; }
    
    public ProductViewModel()
    {
        LoadProductsCommand = new RelayCommand(LoadProducts);
        LoadCategoriesCommand = new RelayCommand(LoadCategories);
        SaveCommand = new RelayCommand(SaveProduct);
        UpdateCommand = new RelayCommand(UpdateProduct);
        DeleteCommand = new RelayCommand(DeleteProduct);
        ClearFormCommand = new RelayCommand(ClearForm);
        SearchCommand = new RelayCommand(SearchProducts);
        
        LoadCategories();
        LoadProducts();
    }
    
    private void LoadProducts()
    {
        try
        {
            IsLoading = true;
            Products = _productRepo.GetAll();
            StatusMessage = $"{Products.Count} products loaded";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading products: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void LoadCategories()
    {
        try
        {
            var repo = new CategoryRepository();
            Categories = repo.GetAll();
            Console.WriteLine($"Categories loaded: {Categories.Count}");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading categories: {ex.Message}";
        }
    }
    
    private void SearchProducts()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadProducts();
            return;
        }
        
        var filtered = new ObservableCollection<Product>();
        foreach (var product in _productRepo.GetAll())
        {
            if (product.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                filtered.Add(product);
            }
        }
        Products = filtered;
        StatusMessage = $"{Products.Count} products found";
    }
    
    private bool CanSaveProduct()
    {
        return !string.IsNullOrWhiteSpace(Name) && Price > 0;
    }
    
    private void SaveProduct()
    {
        try
        {
            IsLoading = true;
            var product = new Product
            {
                Name = Name,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                Price = Price,
                Stock = Stock,
                CategoryId = CategoryId == 0 ? null : CategoryId
            };
            
            _productRepo.Create(product);
            StatusMessage = "Product created successfully";
            ClearForm();
            LoadProducts();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating product: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanUpdateProduct()
    {
        return SelectedProduct != null && !string.IsNullOrWhiteSpace(Name) && Price > 0;
    }
    
    private void UpdateProduct()
    {
        try
        {
            IsLoading = true;
            SelectedProduct.Name = Name;
            SelectedProduct.Description = string.IsNullOrWhiteSpace(Description) ? null : Description;
            SelectedProduct.Price = Price;
            SelectedProduct.Stock = Stock;
            SelectedProduct.CategoryId = CategoryId == 0 ? null : CategoryId;
            
            _productRepo.Update(SelectedProduct);
            StatusMessage = "Product updated successfully";
            ClearForm();
            LoadProducts();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating product: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanDeleteProduct()
    {
        return SelectedProduct != null;
    }
    
    private void DeleteProduct()
    {
        try
        {
            IsLoading = true;
            _productRepo.Delete(SelectedProduct.Id);
            StatusMessage = "Product deleted successfully";
            ClearForm();
            LoadProducts();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting product: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void ClearForm()
    {
        Name = string.Empty;
        Description = string.Empty;
        Brand = string.Empty;
        Price = 0;
        Stock = 0;
        MinStock = 0;
        CategoryId = null;
        SelectedProduct = null!;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}