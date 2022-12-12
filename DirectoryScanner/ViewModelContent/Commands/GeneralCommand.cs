using System;
using System.Windows.Input;

namespace Client.ViewModelContent.Commands 
{
    public class GeneralCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action _action; //срабатывает при вызове команды


        public GeneralCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true; 
        }

        public void Execute(object? parameter)
        {
            _action();
        }
    }
}
