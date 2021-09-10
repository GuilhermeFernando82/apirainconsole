using System.Threading.Tasks;
using System.Linq;
using Ams.Helper;
using System.IO;
using System.Text;
using Ams.Model;
using Models;
using System;

namespace Ams.Business
{
    class SumPrecipitation
    {
        public void Execute (string input, string output)
        {
            var allfiles = FileHelper.GetInputFiles(input);
            var lastFile = allfiles.LastOrDefault();
            var files = allfiles.SkipLast(1).ToList();
            foreach (var file in files)
            {
                Task.Run(() => GetDataFromFile(file, output));
            }

            GetDataFromFile(lastFile, output);
        }

        private void GetDataFromFile(string inputPath, string outputPath)
        {
            var lines = File.ReadLines(inputPath);
            var output = outputPath + "\\" + FileHelper.GetOutputFilename(inputPath);
            var stream = new FileStream(output, FileMode.Create);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            foreach (var line in lines)
            {
                var result = GetDataFromLine(line);
                writer.WriteLine(result);
                writer.Flush();
            }
        }

        private string GetDataFromLine(string line)
        {
            var city = new City(line);
            var url = city.GetSumPrecipitationUrlRequest();

            if (city.HasError || string.IsNullOrEmpty(url))
            {
                return city.Id + ";arquivo de entrada com erro de formatação";
            }

            var result = GetResult(url);
            return result != null ? GetFinalResult(result, city) : string.Empty;
        }

        private string GetFinalResult(SingleResult<PrecipitationResultItem> result, City item)
        {
            if (result != null && result.ObjectResult != null)
            {
                return DateTime.Now.ToString() + ";" + item.Id + ";" + item.GetStartDate() + ";" + item.GetEndDate() + ";" + result.ObjectResult.Sum;
            }
            else
            {
                return item.Id + ";" + item.GetStartDate() + ";" + item.GetEndDate() + ";" + "; Resultado não encontrado.";
            }
        }

        private SingleResult<PrecipitationResultItem> GetResult(string url)
        {
            try
            {
                return HttpRequestHelper.HttpRequest<SingleResult<PrecipitationResultItem>>(url);
            }
            catch
            {
                return null;
            }
        }
    }
}
