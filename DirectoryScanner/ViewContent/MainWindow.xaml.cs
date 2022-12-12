using Client.ViewModelContent.ViewModels; //точка юзинг
using System.Windows;

namespace Client.ViewContent
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;


        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel; //для удобной работы привязки
        }
    }
}
//вью модел хранит данные необходимые для вью форме 
