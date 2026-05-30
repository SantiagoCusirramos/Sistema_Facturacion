using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class CategoryViewModel : INotifyPropertyChanged
{
    private readonly CategoryRepository _categoryRepo = new();
    
    private ObservableCollection<Category> _categories = new();
    private Category _selectedCategory = null!;
    private string _searchText = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
        
    private string _categoryName = string.Empty;
    private string _categoryDescription = string.Empty;
    
    public string DebugInfo => $"Categories in memory: {Categories.Count}";
    
    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set
        {
            _categories = value; OnPropertyChanged()
                ;
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
                CategoryName = value.Name;
                CategoryDescription = value.Description ?? string.Empty;
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
    
    public string CategoryName
    {
        get => _categoryName;
        set { _categoryName = value; OnPropertyChanged(); }
    }
    
    public string CategoryDescription
    {
        get => _categoryDescription;
        set { _categoryDescription = value; OnPropertyChanged(); }
    }
    
    // Comandos
    public ICommand LoadCategoriesCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFormCommand { get; }
    public ICommand SearchCommand { get; }
    
    public CategoryViewModel()
{
    LoadCategoriesCommand = new RelayCommand(LoadCategories);
    SaveCommand = new RelayCommand(SaveCategory);  // Sin canExecute
    UpdateCommand = new RelayCommand(UpdateCategory);  // Sin canExecute
    DeleteCommand = new RelayCommand(DeleteCategory);  // Sin canExecute
    ClearFormCommand = new RelayCommand(ClearForm);
    SearchCommand = new RelayCommand(SearchCategories);
    
    LoadCategories();
    
    Console.WriteLine("CategoryViewModel initialized - Commands ready");
}
    
    private void LoadCategories()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading categories...";
        
            var loaded = _categoryRepo.GetAll();
            Categories = loaded;
        
            StatusMessage = $"SUCCESS: {Categories.Count} categories loaded";
            Console.WriteLine($"Categories loaded: {Categories.Count}");
        
            if (Categories.Count == 0)
            {
                StatusMessage = "No categories found. Please add some categories.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"ERROR: {ex.Message}";
            Console.WriteLine($"Error loading categories: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void SearchCategories()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadCategories();
            return;
        }
        
        var filtered = new ObservableCollection<Category>();
        foreach (var cat in _categoryRepo.GetAll())
        {
            if (cat.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                filtered.Add(cat);
            }
        }
        Categories = filtered;
        StatusMessage = $"{Categories.Count} Category found";
    }
    
    private bool CanSaveCategory()
    {
        return !string.IsNullOrWhiteSpace(CategoryName);
    }
    
    private void SaveCategory()
    {
        try
        {
            IsLoading = true;
            var category = new Category
            {
                Name = CategoryName,
                Description = string.IsNullOrWhiteSpace(CategoryDescription) ? null : CategoryDescription
            };
            
            _categoryRepo.Create(category);
            StatusMessage = "Category created successfully";
            ClearForm();
            LoadCategories();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanUpdateCategory()
    {
        return SelectedCategory != null && !string.IsNullOrWhiteSpace(CategoryName);
    }
    
    private void UpdateCategory()
    {
        try
        {
            IsLoading = true;
            SelectedCategory.Name = CategoryName;
            SelectedCategory.Description = string.IsNullOrWhiteSpace(CategoryDescription) ? null : CategoryDescription;
            
            _categoryRepo.Update(SelectedCategory);
            StatusMessage = "Category updated successfully";
            ClearForm();
            LoadCategories();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanDeleteCategory()
    {
        return SelectedCategory != null;
    }
    
    private void DeleteCategory()
    {
        try
        {
            IsLoading = true;
            _categoryRepo.Delete(SelectedCategory.Id);
            StatusMessage = "Category successfully removed";
            ClearForm();
            LoadCategories();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Delete error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void ClearForm()
    {
        CategoryName = string.Empty;
        CategoryDescription = string.Empty;
        SelectedCategory = null!;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}