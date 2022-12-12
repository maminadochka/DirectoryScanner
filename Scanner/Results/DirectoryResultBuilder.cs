using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Scanner.Results
{
    public partial class DirectoryResult
    {
        public class Builder
        {
            public Builder? Parent { get; }

            public string FullPath
            {
                get { return _result.FullPath; }
            }

            public int NestedToProcess
            {
                get { return Interlocked.CompareExchange(ref _nestedToProcess, 0, 0); }
            }

            public bool AreFilesSealed
            {
                get { return 1 == Interlocked.CompareExchange(ref _areFilesSealed, 0, 0); }
            }

            public bool WasBuildRequested
            {
                get { return 1 == Interlocked.CompareExchange(ref _wasBuildRequested, 0, 0); }
            }

            private DirectoryResult _result;
            private ConcurrentQueue<DirectoryResult> _nestedDirs;
            private ConcurrentQueue<FileResult> _nestedFiles;
            private int _nestedToProcess;
            private int _areFilesSealed;
            private int _wasBuildRequested;


            public Builder(string directoryPath, Builder? parent)
            {
                _result = new DirectoryResult(directoryPath);
                Parent = parent;
                _nestedDirs = new ConcurrentQueue<DirectoryResult>();
                _nestedFiles = new ConcurrentQueue<FileResult>();
                _areFilesSealed = 0;
                _wasBuildRequested = 0;

                if (Parent != null)
                {
                    Interlocked.Increment(ref Parent._nestedToProcess);
                }
            }

            public Builder(string directoryPath)
                : this(directoryPath, null)
            {

            }


            public DirectoryResult Build()
            {
                Interlocked.Exchange(ref _wasBuildRequested, 1);
                _result._nestedDirs = _nestedDirs.ToList();
                _result._nestedFiles = _nestedFiles.ToList();
                _result.NestedDirs = new ReadOnlyCollection<DirectoryResult>(_result._nestedDirs);
                _result.NestedFiles = new ReadOnlyCollection<FileResult>(_result._nestedFiles);

                if (Parent != null)
                {
                    Parent._nestedDirs.Enqueue(_result);
                    Interlocked.Add(ref Parent._result._dirsSize, _result._dirsSize);
                    Interlocked.Add(ref Parent._result._dirsSize, _result._filesSize);
                    Interlocked.Decrement(ref Parent._nestedToProcess);
                }

                return _result;
            }

            public void AppendFileResult(FileResult fileResult)
            {
                _nestedFiles.Enqueue(fileResult);
                Interlocked.Add(ref _result._filesSize, fileResult.Size);
            }

            public void SeadFileResults()
            {
                Interlocked.Exchange(ref _areFilesSealed, 1);
            }
        }
    }
}
