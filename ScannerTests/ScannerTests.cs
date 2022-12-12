using Xunit;
using Scanner;
using System.IO.Abstractions;
using ScannerTests.Helpers;

namespace ScannerTests
{
    public class ScannerTests
    {
        [Fact]
        public void Start_DirectoryNotFound()
        {
            //arrange
            IFileSystem fileSystem = FileSystemHelper.CreateFileSystemMock();
            DirScanner scanner = new DirScanner(fileSystem, 10);

            //act + assert
            Assert.Throws<DirectoryNotFoundException>(() => scanner.Start("this_dir_does_not_exit"));
        }

        [Fact]
        public void Constructor_ThreadsLimitOutOfRange()
        {
            //arrange
            IFileSystem fileSystem = FileSystemHelper.CreateFileSystemMock();

            //act + assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new DirScanner(fileSystem, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DirScanner(fileSystem, 0));
        }

        [Fact]
        public void GetResults_Null()
        {
            //arrange
            IFileSystem fileSystem = FileSystemHelper.CreateFileSystemMock();
            DirScanner scanner = new DirScanner(fileSystem, 30);

            //act
            var res = scanner.GetResult();

            //assert
            Assert.Null(res);
        }

        [Fact]
        public void GetResults_NotNull()
        {
            //arrange
            IFileSystem fileSystem = FileSystemHelper.CreateFileSystemMock();
            DirScanner scanner = new DirScanner(fileSystem, 30);

            //act
            scanner.Start("K:\\Dir_A");
            var res = scanner.GetResult();

            //assert
            Assert.NotNull(res);
        }
    }
}
