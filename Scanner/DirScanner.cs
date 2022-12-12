using Scanner.Results;
using System.Collections.Concurrent;
using System.IO.Abstractions;

namespace Scanner
{
    public class DirScanner
    {
        public int ThreadsLimit { get; }

        public bool IsRunning
        {
            get { return 1 == Interlocked.CompareExchange(ref _isRunning, 0, 0); } 
        }

        private int _isRunning;

        private Semaphore _semaphore;

        private ConcurrentQueue<DirectoryResult.Builder> _dirsToProcess;

        private CancellationTokenSource _cancellationTokenSource;

        private DirectoryResult? _result;

        private IFileSystem _fileSystem;


        public DirScanner(IFileSystem fileSystem, int threadsLimit)
        {
            if (threadsLimit <= 0)
                throw new ArgumentOutOfRangeException();

            _fileSystem = fileSystem;
            ThreadsLimit = threadsLimit;
            _cancellationTokenSource = new CancellationTokenSource();
            _dirsToProcess = new ConcurrentQueue<DirectoryResult.Builder>();
            _semaphore = new Semaphore(ThreadsLimit, ThreadsLimit);
            _isRunning = 0;
        }


        public void Start(string path)
        {
            if (!_fileSystem.Directory.Exists(path))
                throw new DirectoryNotFoundException();

            Interlocked.Exchange(ref _isRunning, 1); 
            var builder = new DirectoryResult.Builder(path); 
            _dirsToProcess.Enqueue(builder); 

            while (IsRunning)
            {
                if (_dirsToProcess.TryDequeue(out var w)) 
                {
                    _semaphore.WaitOne(); 
                    ThreadPool.QueueUserWorkItem((o) => ProcessDirectory(w), _cancellationTokenSource.Token, false);
                }                                                              
            }
        }

        private void ProcessDirectory(DirectoryResult.Builder builder)
        {
            Thread.Sleep(100); 

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                EnqueueNestedDirectories(builder);
                CalculateNestedFiles(builder); 
                BuildHierarchy(builder); 
            }

            if (_cancellationTokenSource.IsCancellationRequested)
            {
                var currBuilder = builder;
                while (currBuilder.Parent != null && !currBuilder.Parent.WasBuildRequested)
                {
                    currBuilder.Build();
                    currBuilder = currBuilder.Parent;
                }

                if (currBuilder.Parent == null)
                {
                    _result = currBuilder.Build();
                    Interlocked.Exchange(ref _isRunning, 0);
                }
            }

            _semaphore.Release(); 
        }

        private void EnqueueNestedDirectories(DirectoryResult.Builder builder) 
        {
            foreach (var dir in _fileSystem.Directory.GetDirectories(builder.FullPath))
            {
                var nestedBuilder = new DirectoryResult.Builder(dir, builder);
                _dirsToProcess.Enqueue(nestedBuilder);
            }
        }

        private void CalculateNestedFiles(DirectoryResult.Builder builder)
        {
            foreach (var file in _fileSystem.DirectoryInfo.FromDirectoryName(builder.FullPath).GetFiles())
            {
                if (file.LinkTarget == null)
                {
                    var fileResult = new FileResult(file.Name, file.Length);
                    builder.AppendFileResult(fileResult);
                }
            }

            builder.SeadFileResults(); 
        }

        private void BuildHierarchy(DirectoryResult.Builder builder)
        {
            var currBuilder = builder;
            while (currBuilder.AreFilesSealed && (currBuilder.NestedToProcess == 0)) 
            {//директория полностью обработана
                if (currBuilder.Parent == null)
                {
                    _result = currBuilder.Build();
                    Interlocked.Exchange(ref _isRunning, 0);
                    break;
                }
                currBuilder.Build(); 
                currBuilder = currBuilder.Parent; 
            }
        } 

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public DirectoryResult? GetResult() 
        {
            if (IsRunning)
                throw new InvalidOperationException();

            return _result;
        }

        public DirectoryResult? WaitResult() 
        {
            while (IsRunning) { };

            return _result;
        }
    }
}
