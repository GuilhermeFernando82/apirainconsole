using Ams.Business;
using System;
using System.Threading;

namespace Ams
{
    public class Program
    {
        protected Program()
        {

        }

        static void Main(string[] args)
        {
            if(args.Length != 3)
            {
                Console.WriteLine("Precisa informar 3 parâmetros (separados por espaço): \ntipo de operação, que pode ser: breakoperation; sumprecipitation; rainfallforecast\ndiretório de entrada (entre aspas duplas); \ndiretório de saída (entre aspas duplas);");
                return;
            }

            var type = args[0];
            var input = args[1];
            var output = args[2];

            Console.WriteLine("Iniciando processo as " + DateTime.Now.ToString());
            switch (type)
            {
                case "breakoperation":
                    var breakBo = new BreakProduction();
                    breakBo.Execute(input, output);
                    break;
                case "sumprecipitation":
                    var precipitationBo = new SumPrecipitation();
                    precipitationBo.Execute(input, output);
                    break;
                case "rainfallforecast":
                    var forecastBo = new Forecast120();
                    forecastBo.Execute(input, output);
                    break;
                default:
                    Console.WriteLine("Operação " + type + " não permitida.");
                    break;
            }
            Console.WriteLine("Concluindo processo as " + DateTime.Now.ToString());
            Thread.Sleep(5000);
        }
    }
}
