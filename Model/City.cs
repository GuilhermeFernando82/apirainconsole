using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ams.Model
{
    class City
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool HasError { get; set; }

        public City(string line)
        {
            var items = line.Split(";");
            if (items.Length != 5)
            {
                HasError = true;
                return;
            }

            try
            {
                Id = Int32.Parse(items[0]);
                Latitude = items[1];
                Longitude = items[2];
                StartDate = Convert.ToDateTime(items[3], new CultureInfo("en-US"));
                EndDate = Convert.ToDateTime(items[4], new CultureInfo("en-US"));
                HasError = false;
            }
            catch
            {
                HasError = true;
            }
        }

        internal string GetSumPrecipitationUrlRequest()
        {
            if (HasError)
            {
                return string.Empty;
            }

            var format = "https://precipitation.atfunctions.com/api/precipitation/get_sum_by_point/{0}/{1}/{2}/{3}/?IAMKEY=AgzhY3hqqzeSt97DNQMiuA2bn1xIdv6UEu8H18tdb_EaoGMJVh-D5se4Y4Y0FcqF";
            var url = new StringBuilder();
            url.AppendFormat(format, Longitude, Latitude, GetStartDate(), GetEndDate());

            return url.ToString();
        }

        internal string GetForecastUrlRequest()
        {
            if (HasError)
            {
                return string.Empty;
            }

            var format = "https://forecast120.atfunctions.com/api/forecast120/get_by_point/{0}/{1}/{2}/{3}/?IAMKEY=AgzhY3hqqzeSt97DNQMiuA2bn1xIdv6UEu8H18tdb_EaoGMJVh-D5se4Y4Y0FcqF";
            var url = new StringBuilder();
            url.AppendFormat(format, Longitude, Latitude, GetStartDate(), GetEndDate());

            return url.ToString();
        }

        public string GetEndDate()
        {
            return EndDate.Year + "-" + EndDate.Month + "-" + EndDate.Day;
        }

        public string GetStartDate()
        {
            return StartDate.Year + "-" + StartDate.Month + "-" + StartDate.Day;
        }
    }
}
