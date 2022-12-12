namespace Scanner.Results
{
    public class FileResult
    {
        public string FileName { get; }

        public long Size { get; }


        public FileResult(string fileName, long size)
        {
            FileName = fileName;
            Size = size;
        }
    }
}
