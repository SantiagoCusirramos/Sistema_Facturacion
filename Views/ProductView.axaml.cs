using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class ProductView : UserControl
{
    public ProductView()
    {
        InitializeComponent();
        DataContext = new ProductViewModel();
    }
}