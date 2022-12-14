using System.ComponentModel;
using System.Runtime.CompilerServices;
//абстрактная модель
namespace Client.ViewModelContent.ViewModels.Abstract
{
    public class ViewModelBase : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
