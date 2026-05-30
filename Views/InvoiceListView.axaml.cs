using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class InvoiceListView : UserControl
{
    public InvoiceListView()
    {
        InitializeComponent();
        DataContext = new InvoiceListViewModel();
    }
}