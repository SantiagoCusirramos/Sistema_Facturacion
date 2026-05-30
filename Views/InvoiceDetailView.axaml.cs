using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class InvoiceDetailView : UserControl
{
    private InvoiceDetailViewModel? ViewModel => DataContext as InvoiceDetailViewModel;
    
    public InvoiceDetailView()
    {
        InitializeComponent();
        DataContext = new InvoiceDetailViewModel();
    }
    
    public void LoadInvoice(int invoiceId)
    {
        ViewModel?.SetInvoiceId(invoiceId);
    }
}