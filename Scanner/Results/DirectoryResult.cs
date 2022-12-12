using System.Collections.ObjectModel;

namespace Scanner.Results
{
    public partial class DirectoryResult
    {
        public string FullPath { get; }

        public long FilesSize
        {
            get { return _filesSize; }
            private set { _filesSize = value; }
        }

        public long DirsSize
        {
            get { return _dirsSize; }
            private set { _dirsSize = value; }
        }

        public long TotalSize
        {
            get { return FilesSize + DirsSize; }
        }
        
        public ReadOnlyCollection<DirectoryResult> NestedDirs { get; private set; }

        public ReadOnlyCollection<FileResult> NestedFiles { get; private set; }

        private long _filesSize;
        private long _dirsSize;
        private List<DirectoryResult> _nestedDirs;
        private List<FileResult> _nestedFiles;


        private DirectoryResult(string fullpath)
        {
            FullPath = fullpath;
        }
    }
}
