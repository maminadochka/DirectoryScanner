using Client.ViewModelContent.ViewModels.Abstract;
using Scanner.Results;
using System.Collections.ObjectModel;

namespace Client.ViewModelContent.ViewModels
{
    public class DirectoryResultViewModel: NodeViewModel
    {
        public ReadOnlyObservableCollection<NodeViewModel> Children { get; }    

        private ObservableCollection<NodeViewModel> _children;


        private DirectoryResultViewModel(string name, long size, long? parentSize)
            : base(name, size, parentSize)
        {
            _children = new ObservableCollection<NodeViewModel>();
            Children = new ReadOnlyObservableCollection<NodeViewModel>(_children);
        }

                                                                                //возможный родитель
        public static DirectoryResultViewModel Create(DirectoryResult dirResult, DirectoryResult? parent)
        {
            DirectoryResultViewModel viewModel = null!;
            if (parent != null)
                viewModel = new DirectoryResultViewModel(dirResult.FullPath, dirResult.TotalSize, parent.TotalSize);
            else
                viewModel = new DirectoryResultViewModel(dirResult.FullPath, dirResult.TotalSize, null);

            foreach (var file in dirResult.NestedFiles)
            {
                var fileViewModel = FileResultViewModel.Create(file, dirResult);
                viewModel._children.Add(fileViewModel);
            }

            foreach (var dir in dirResult.NestedDirs)
            {
                var dirViewModel = Create(dir, dirResult); 
                viewModel._children.Add(dirViewModel);
            }

            return viewModel;
        }
    }
}
