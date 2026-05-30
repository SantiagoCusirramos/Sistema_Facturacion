using System;
using System.Windows.Input;

namespace Sistema_Facturacion.Helpers;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    private EventHandler? _canExecuteChanged;
    
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute();
    }
    
    public void Execute(object? parameter)
    {
        _execute();
    }
    
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            _canExecuteChanged += value;
            CommandManagerRequerySuggested?.Invoke(this, EventArgs.Empty);
        }
        remove
        {
            _canExecuteChanged -= value;
        }
    }
    
    private event EventHandler? CommandManagerRequerySuggested;
    
    public void RaiseCanExecuteChanged()
    {
        var handler = _canExecuteChanged;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
        
        CommandManagerRequerySuggested?.Invoke(this, EventArgs.Empty);
    }
}