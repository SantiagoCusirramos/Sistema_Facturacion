using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class CustomerView : UserControl
{
    public CustomerView()
    {
        InitializeComponent();
        DataContext = new CustomerViewModel();
    }
}