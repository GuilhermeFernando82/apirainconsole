using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ams
{
    public class Gpm
    {
        public string Hash { get; set; }
        public string Culture { get; set; }
        public DateTime PlantingDate { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool HasError { get; set; }
        public int Tenth
        {
            get
            {
                if (PlantingDate.Day < 11)
                    return 1;

                if (PlantingDate.Day < 21)
                    return 2;

                return 3;
            }
        }

        public Gpm(string line)
        {
            var items = line.Split(";");
            if (items.Length != 5)
            {
                HasError = true;
                return;
            }

            try
            {
                Hash = items[0];
                Culture = items[1];
                PlantingDate = Convert.ToDateTime(items[2], new CultureInfo("en-US"));
                Longitude = items[3];
                Latitude = items[4];
                HasError = false;
            }
            catch
            {
                HasError = true;
            }
        }

        internal string GetUrlRequest()
        {
            if (HasError)
            {
                return string.Empty;
            }

            var format = "https://ams.atfunctions.com/api/ams/simulate/{0}/{1}/1/{2}/{3}/{4}/?IAMKEY=AgzhY3hqqzeSt97DNQMiuA2bn1xIdv6UEu8H18tdb_EaoGMJVh-D5se4Y4Y0FcqF";
            var url = new StringBuilder();
            url.AppendFormat(format, Longitude, Latitude, Culture, GetMonth(), GetYear());

            return url.ToString();
        }

        public int GetYear()
        {
            return PlantingDate.Year;
        }

        public int GetMonth()
        {
            return PlantingDate.Month;
        }
    }
}
