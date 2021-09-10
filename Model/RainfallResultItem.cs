using System;
using System.Collections.Generic;
using System.Text;

namespace Ams.Model
{
    class RainfallResultItem
    {
        public List<RailfallForecastList> ForecastList { get; set; }
    }

    internal class RailfallForecastList
    {
        public DateTime Day { get; set; }
        public Double Rainfall_Value { get; set; }
        public Double Temperature_Value { get; set; }
    }
}
