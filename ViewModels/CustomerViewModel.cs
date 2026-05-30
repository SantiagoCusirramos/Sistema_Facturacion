using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class CustomerViewModel : INotifyPropertyChanged
{
    private readonly CustomerRepository _customerRepo = new();
    
    private ObservableCollection<Customer> _customers = new();
    private Customer _selectedCustomer = null!;
    private string _searchText = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
    private string _documentId = string.Empty;
    private string _name = string.Empty;
    private string _address = string.Empty;
    private string _phone = string.Empty;
    private string _email = string.Empty;
    
    public ObservableCollection<Customer> Customers
    {
        get => _customers;
        set { _customers = value; OnPropertyChanged(); }
    }
    
    public Customer SelectedCustomer
    {
        get => _selectedCustomer;
        set 
        { 
            _selectedCustomer = value; 
            OnPropertyChanged();
            if (value != null)
            {
                DocumentId = value.DocumentId;
                Name = value.Name;
                Address = value.Address ?? string.Empty;
                Phone = value.Phone ?? string.Empty;
                Email = value.Email ?? string.Empty;
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
    
    public string DocumentId
    {
        get => _documentId;
        set { _documentId = value; OnPropertyChanged(); }
    }
    
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }
    
    public string Address
    {
        get => _address;
        set { _address = value; OnPropertyChanged(); }
    }
    
    public string Phone
    {
        get => _phone;
        set { _phone = value; OnPropertyChanged(); }
    }
    
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }
    
    public ICommand LoadCustomersCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFormCommand { get; }
    public ICommand SearchCommand { get; }
    
    public CustomerViewModel()
    {
        LoadCustomersCommand = new RelayCommand(LoadCustomers);
        SaveCommand = new RelayCommand(SaveCustomer);
        UpdateCommand = new RelayCommand(UpdateCustomer);
        DeleteCommand = new RelayCommand(DeleteCustomer);
        ClearFormCommand = new RelayCommand(ClearForm);
        SearchCommand = new RelayCommand(SearchCustomers);
        
        LoadCustomers();
        
        Console.WriteLine("CategoryViewModel initialized - Commands ready");
    }
    
    private void LoadCustomers()
    {
        try
        {
            IsLoading = true;
            Customers = _customerRepo.GetAll();
            StatusMessage = $"{Customers.Count} customers loaded";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading customers: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void SearchCustomers()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadCustomers();
            return;
        }
        
        var filtered = new ObservableCollection<Customer>();
        foreach (var customer in _customerRepo.GetAll())
        {
            if (customer.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                customer.DocumentId.Contains(SearchText))
            {
                filtered.Add(customer);
            }
        }
        Customers = filtered;
        StatusMessage = $"{Customers.Count} customers found";
    }
    
    private bool CanSaveCustomer()
    {
        return !string.IsNullOrWhiteSpace(DocumentId) && !string.IsNullOrWhiteSpace(Name);
    }
    
    private void SaveCustomer()
    {
        try
        {
            IsLoading = true;
            var customer = new Customer
            {
                DocumentId = DocumentId,
                Name = Name,
                Address = string.IsNullOrWhiteSpace(Address) ? null : Address,
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone,
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email
            };
            
            _customerRepo.Create(customer);
            StatusMessage = "Customer created successfully";
            ClearForm();
            LoadCustomers();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating customer: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanUpdateCustomer()
    {
        return SelectedCustomer != null && !string.IsNullOrWhiteSpace(Name);
    }
    
    private void UpdateCustomer()
    {
        try
        {
            IsLoading = true;
            SelectedCustomer.DocumentId = DocumentId;
            SelectedCustomer.Name = Name;
            SelectedCustomer.Address = string.IsNullOrWhiteSpace(Address) ? null : Address;
            SelectedCustomer.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone;
            SelectedCustomer.Email = string.IsNullOrWhiteSpace(Email) ? null : Email;
            
            _customerRepo.Update(SelectedCustomer);
            StatusMessage = "Customer updated successfully";
            ClearForm();
            LoadCustomers();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating customer: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanDeleteCustomer()
    {
        return SelectedCustomer != null;
    }
    
    private void DeleteCustomer()
    {
        try
        {
            IsLoading = true;
            _customerRepo.Delete(SelectedCustomer.Id);
            StatusMessage = "Customer deleted successfully";
            ClearForm();
            LoadCustomers();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting customer: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void ClearForm()
    {
        DocumentId = string.Empty;
        Name = string.Empty;
        Address = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
        SelectedCustomer = null!;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}