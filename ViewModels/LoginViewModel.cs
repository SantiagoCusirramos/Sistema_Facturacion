using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.ViewModels;

public class LoginViewModel
{
    private readonly AppUserRepository _userRepo = new();
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading = false;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Username
    {
        get => _username;
        set
        {
            _username = value; 
            // OnPropertyChanged();
        }
    }
    
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            // OnPropertyChanged();
        }
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value; 
            // OnPropertyChanged();
        }
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value; 
            // OnPropertyChanged();
        }
    }
    
    public ICommand LoginCommand { get; }
    public ICommand ExitCommand { get; }
    
    public event EventHandler? LoginSuccess;

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        ExitCommand = new RelayCommand(ExecuteExit);
    }

    private bool CanExecuteLogin()
    {
        return !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    }

    private async void ExecuteLogin()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            await Task.Run(() =>
            {
                var user = _userRepo.GetByUsername(Username);
                if (user == null)
                {
                    ErrorMessage = $"User {Username} not found";
                }
                else if (user.PasswordHash != Password)
                {
                    ErrorMessage = $"User {Username} has incorrect password";
                }
                else if (!user.IsActive)
                {
                    ErrorMessage = $"User {Username} is not active";
                }
                else
                {
                    LoginSuccess?.Invoke(this, EventArgs.Empty);

                }
            });
            
        }
        catch (Exception e)
        {
            ErrorMessage = $"Error:  {e.Message }";
            Console.WriteLine(e);
            throw;
        } finally{
            IsLoading = false;
        }
    }

    private void ExecuteExit()
    {
        Environment.Exit(0);
    }

    protected void OnPropertyChanged([CallerMemberName] String? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    
}