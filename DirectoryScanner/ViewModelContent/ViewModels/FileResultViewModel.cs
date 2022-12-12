using Client.ViewModelContent.ViewModels.Abstract;
using Scanner.Results;

namespace Client.ViewModelContent.ViewModels
{
    public class FileResultViewModel : NodeViewModel
    {
        private FileResultViewModel(string name, long size, long parentSize) 
            : base(name, size, parentSize)
        {

        }


        public static FileResultViewModel Create(FileResult fileResult, DirectoryResult parent)
        {
            return new FileResultViewModel(fileResult.FileName, fileResult.Size, parent.TotalSize);
        }
    }
}
