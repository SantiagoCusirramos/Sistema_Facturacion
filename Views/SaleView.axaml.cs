using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class SaleView : UserControl
{
    public SaleView()
    {
        InitializeComponent();
        DataContext = new SaleViewModel();
    }
}