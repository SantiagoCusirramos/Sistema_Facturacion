using Avalonia.Controls;
using Avalonia.Threading;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class LoginView : Window
{
    private LoginViewModel? ViewModel => DataContext as LoginViewModel;
    
    public LoginView()
    {
        InitializeComponent();
        DataContext = new LoginViewModel();
        
        if (ViewModel != null)
        {
            ViewModel.LoginSuccess += OnLoginSuccess;
        }
    }
    
    private async void OnLoginSuccess(object? sender, System.EventArgs e)
    {
        // Usar Dispatcher para asegurar que se ejecuta en el hilo UI
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        });
    }
}