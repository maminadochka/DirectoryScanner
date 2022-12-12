using Client.ViewModelContent.Commands;
using Ookii.Dialogs.Wpf;
using Client.ViewModelContent.ViewModels.Abstract;
using Scanner;
using System.IO.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using Scanner.Results;
using System.Linq;

namespace Client.ViewModelContent.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string SelectedPath 
        {
            get { return _selectedPath; }
            set 
            {
                _selectedPath = value;
                NotifyPropertyChanged(nameof(SelectedPath));
            }
        }

        public ObservableCollection<NodeViewModel> Children 
        {
            get { return _children; }
            set
            {
                _children = value;
                NotifyPropertyChanged(nameof(Children));
            }
        }

        //команды для уведомления
        public GeneralCommand StartCommand { get; } 

        public GeneralCommand CancelCommand { get; }

        public GeneralCommand BrowsePathCommand { get; }

        private string _selectedPath;
        private DirScanner _scanner;
        private ObservableCollection<NodeViewModel> _children;


        public MainViewModel()
        {
            StartCommand = new GeneralCommand(Start);
            CancelCommand = new GeneralCommand(Cancel);
            BrowsePathCommand = new GeneralCommand(BrowsePath); 
            SelectedPath = string.Empty;
            _scanner = new DirScanner(new FileSystem(), 30);
            _children = new ObservableCollection<NodeViewModel>();
        }


        private void Start() 
        {
            if (CanBeStarted()) 
            {
                DirectoryResult? result = null; 
                string path = string.Copy(_selectedPath);
                
                Thread scanThread = new Thread(() =>
                {
                    _scanner.Start(path);
                    result = _scanner.WaitResult();

                    if (result != null)
                    {
                        var viewModel = DirectoryResultViewModel.Create(result, null);
                        
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Children.Clear();
                            Children.Add(viewModel);
                        });
                    }
                });

                scanThread.IsBackground = true;
                scanThread.Start();
            }
        }

        private void Cancel()
        {
            if (CanBeCancelled())
            {
                _scanner.Stop();
            }
        }

        private void BrowsePath() 
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                SelectedPath = dialog.SelectedPath;
            }
        }

        private bool CanBeStarted() 
        {
            return !string.IsNullOrEmpty(SelectedPath) && !_scanner.IsRunning;
        }

        private bool CanBeCancelled()
        {
            return _scanner.IsRunning;
        }
    }
}
