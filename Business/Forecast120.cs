using Ams.Helper;
using Ams.Model;
using Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ams.Business
{
    class Forecast120
    {
        public void Execute(string input, string output)
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
            var rainfallStream = new FileStream(output.Replace(".txt", "_rainfall.txt"), FileMode.Create);
            var temperatureStream = new FileStream(output.Replace(".txt", "_temperature.txt"), FileMode.Create);

            using var rainfallWriter = new StreamWriter(rainfallStream, Encoding.UTF8);
            using var temperatureWriter = new StreamWriter(temperatureStream, Encoding.UTF8);
            foreach (var line in lines)
            {
                var result = GetDataFromLine(line);

                rainfallWriter.WriteLine(result.Rainfall);
                temperatureWriter.WriteLine(result.Temperature);

                rainfallWriter.Flush();
                temperatureWriter.Flush();
            }
        }

        private ForecastLineResult GetDataFromLine(string line)
        {
            var city = new City(line + ";2020-01-01;2020-01-01");
            city.StartDate = DateTime.Now;
            city.EndDate = DateTime.Now.AddDays(120);

            var url = city.GetForecastUrlRequest();
            if (city.HasError || string.IsNullOrEmpty(url))
            {
                return new ForecastLineResult()
                {
                    Rainfall = city.Id + ";arquivo de entrada com erro de formatação",
                    Temperature = city.Id + ";arquivo de entrada com erro de formatação"
                };
            }

            var result = GetResult(url);
            return result != null ? GetFinalResult(result, city) : new ForecastLineResult();
        }

        private ForecastLineResult GetFinalResult(SingleResult<RainfallResultItem> result, City item)
        {
            if (result != null && result.ObjectResult != null && result.ObjectResult.ForecastList != null && result.ObjectResult.ForecastList.Count > 0)
            {
                var rainfallLine = new StringBuilder();
                var temperatureLine = new StringBuilder();
                var month = 0;
                var year = 0;
                var day = 0;
                foreach (var forecast in result.ObjectResult.ForecastList)
                {
                    var value = $"{DateTime.Now.ToString()};{item.Id};{forecast.Day.Year};{forecast.Day.Month};{GetDifferenceDays(forecast.Day.Day, true)}";
                    if (month == 0)
                    {
                        rainfallLine.Append(value);
                        temperatureLine.Append(value);
                    }
                    else if (month != forecast.Day.Month || year != forecast.Day.Year)
                    {
                        rainfallLine.Append($"{GetDifferenceDays(day, false)}\n{value}");
                        temperatureLine.Append($"{GetDifferenceDays(day, false)}\n{value}");
                    }
                    else
                    {
                        rainfallLine.Append(";");
                        temperatureLine.Append(";");
                    }

                    rainfallLine.Append($"{forecast.Rainfall_Value}");
                    temperatureLine.Append($"{forecast.Temperature_Value}");
                    day = forecast.Day.Day;
                    month = forecast.Day.Month;
                    year = forecast.Day.Year;
                }
                rainfallLine.Append($"{GetDifferenceDays(day, false)}");
                temperatureLine.Append($"{GetDifferenceDays(day, false)}");
                return new ForecastLineResult()
                {
                    Rainfall = rainfallLine.ToString(),
                    Temperature = temperatureLine.ToString()
                };
            }
            else
            {
                return new ForecastLineResult()
                {
                    Rainfall = item.Id + ";" + item.GetStartDate() + ";" + item.GetEndDate() + ";" + "; Resultado não encontrado.",
                    Temperature = item.Id + ";" + item.GetStartDate() + ";" + item.GetEndDate() + ";" + "; Resultado não encontrado."
                };
            }
        }

        private object GetDifferenceDays(int day, bool initial)
        {
            var result = new StringBuilder();
            var diffDays = Math.Abs(day - (initial ? 1 : 31));
            var value = initial ? "null;" : ";null";
            
            for (var i = 0; i < diffDays; i++)
                result.Append(value);

            return result.ToString();
        }

        private SingleResult<RainfallResultItem> GetResult(string url)
        {
            try
            {
                return HttpRequestHelper.HttpRequest<SingleResult<RainfallResultItem>>(url);
            }
            catch
            {
                return null;
            }
        }
    }
}
