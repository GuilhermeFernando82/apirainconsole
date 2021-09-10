using System.IO;

namespace Ams.Helper
{
    public static class FileHelper
    {
        public static string[] GetInputFiles(string path)
        {
            return Directory.GetFiles(path, "*.txt");
        }

        public static string GetOutputFilename(string inputPath)
        {
            return "output_" + inputPath.Substring(inputPath.LastIndexOf("\\") + 1);
        }
    }
}
