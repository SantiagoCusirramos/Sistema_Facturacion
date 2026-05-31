using Avalonia.Controls;
using Sistema_Facturacion.ViewModels;

namespace Sistema_Facturacion.Views;

public partial class ReportView : UserControl
{
    public ReportView()
    {
        InitializeComponent();
        DataContext = new ReportViewModel();
    }
}