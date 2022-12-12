using Scanner.Results;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

namespace ScannerTests.Helpers
{
    public static class FileSystemHelper
    {
        private static string dir_A = "K:\\Dir_A";
        private static string dir_AA = dir_A + "\\Dir_AA";
        private static string dir_AB = dir_A + "\\Dir_AB";

        private static string dir_A_file_A_Path = dir_A + "\\file_A";
        private static string dir_A_file_A_Content = "The quick brown fox jumps over the lazy dog";
        private static Encoding dir_A_file_A_Encoding = Encoding.UTF8;

        private static string dir_AA_file_A_Path = dir_AA + "\\file_A";
        private static string dir_AA_file_A_Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit...";
        private static Encoding dir_AA_file_A_Encoding = Encoding.UTF32;

        private static string dir_AB_file_A_Path = dir_AB + "\\file_A";
        private static string dir_AB_file_A_Content = "Hello there!";
        private static Encoding dir_AB_file_A_Encoding = Encoding.Unicode;


        public static IFileSystem CreateFileSystemMock()
        {
            MockFileData dir_A_file_A = new MockFileData(dir_A_file_A_Content, dir_A_file_A_Encoding);
            MockFileData dir_AA_file_A = new MockFileData(dir_AA_file_A_Content, dir_AA_file_A_Encoding);
            MockFileData dir_AB_file_A = new MockFileData(dir_AB_file_A_Content, dir_AB_file_A_Encoding);

            var res = new MockFileSystem();
            res.AddDirectory(dir_A);
            res.AddDirectory(dir_AA);
            res.AddDirectory(dir_AB);
            res.AddFile(dir_A_file_A_Path, dir_A_file_A_Content);
            res.AddFile(dir_AA_file_A_Path, dir_AA_file_A);
            res.AddFile(dir_AB_file_A_Path, dir_AB_file_A);

            return res;
        }
    }
}
