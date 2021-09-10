using Ams.Helper;
using Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ams.Business
{
    class BreakProduction
    {
        public void Execute(string input, string output)
        {
            var allfiles = FileHelper.GetInputFiles(input);
            var lastFile = allfiles.LastOrDefault();
            var files = allfiles.SkipLast(1).ToList();
            foreach (var file in files)
            {
                Task.Run(() => GetAmsDataFromFile(file, output));
            }

            GetAmsDataFromFile(lastFile, output);
        }

        private void GetAmsDataFromFile(string inputPath, string outputPath)
        {
            var lines = File.ReadLines(inputPath);
            var output = outputPath + "\\" + FileHelper.GetOutputFilename(inputPath);
            var stream = new FileStream(output, FileMode.Create);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            foreach (var line in lines)
            {
                var result = GetAmsDataFromLine(line);
                writer.WriteLine(result);
                writer.Flush();
            }
        }

        private string GetAmsDataFromLine(string line)
        {
            var gpm = new Gpm(line);
            var url = gpm.GetUrlRequest();

            if (gpm.HasError || string.IsNullOrEmpty(url))
            {
                return gpm.Hash + ";arquivo de entrada com erro de formatação";
            }

            var result = GetAmsResult(url);
            return result != null ? GetFinalResult(result, gpm) : string.Empty;
        }

        private string GetFinalResult(ListResult<AmsResultItem> result, Gpm gpm)
        {
            var items = result.ObjectResult.Where(e => e.Month == gpm.GetMonth() && e.Year == gpm.GetYear()).OrderBy(e => e.TenthIndex).ToList();
            if (items.Count >= gpm.Tenth)
            {
                return DateTime.Now.ToString() + ";" + gpm.Hash + ";" + gpm.Culture + ";" + items[gpm.Tenth - 1].Break;
            }
            else
            {
                return gpm.Hash + "; Resultado não encontrado para o " + gpm.Tenth + " decendio.";
            }
        }

        private ListResult<AmsResultItem> GetAmsResult(string url)
        {
            try
            {
                return HttpRequestHelper.HttpRequest<ListResult<AmsResultItem>>(url);
            }
            catch
            {
                return null;
            }
        }
    }
}
