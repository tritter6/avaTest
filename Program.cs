using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Get data from CSV file created by downloading given data. File is placed into application run folder - normally would be API call or DB
                string filename = "27612-precipitation-data.csv";
                //Read date and precipitation amt into list by parsing csv file
                List<cPrecipdata> precipList = ReadData(filename);
                DateTime dateRequested = DateTime.Now;

                //for specific date
                if (args.Length > 0)
                {
                    DateTime trydate = DateTime.MinValue;
                    if (DateTime.TryParse(args[0], out trydate))
                        dateRequested = trydate;
                    else
                        throw new Exception("ERROR: Invalid date supplied.");
                }
                //Does not deal with leap year. Does not deal with days not recorded. Could possibly use 'day of the year'
                //Could extrapolate between days but probably inaccurate.
                //Calculate avg amount and display as predicted value.
                var avgAmt = precipList.Where(x => x.date.Month == dateRequested.Month && x.date.Day == dateRequested.Day).Select(y => y.amt).Average();

                Console.WriteLine("Predicted precipitation for " + dateRequested.ToShortDateString() + " is " + (avgAmt ?? 0).ToString("0.000") + " inches");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Reads given csv file in proper format into list
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>List<cPrecipdata></returns>
        private static List<cPrecipdata> ReadData(string filename)
        {
            bool firstLine = true;
            string line = "";

            try
            {
                List<cPrecipdata> pdList = new List<cPrecipdata>();

                using (StreamReader sr = new StreamReader(filename))
                {
                    //read file line by line - inefficient
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (firstLine) // skip header line
                        {
                            firstLine = false;
                            continue;
                        }

                        //parse line skipping past undesired commas
                        int x = line.LastIndexOf("-");
                        string newLine = line.Substring(x);
                        string[] data = newLine.Split(",");
                        DateTime d = DateTime.MinValue;
                        decimal? amt = 0;

                        if (data.Length >= 3)
                        {
                            if (!DateTime.TryParse(data[2], out d))
                                continue; //Without a date it is useless
                        }
                        if (data.Length == 4)
                        {
                            if (data[3] != null & data[3] != "")
                                amt = Convert.ToDecimal(data[3]);
                            else //Sets amt to 0, but probably should skip this record
                                ;
                        }

                        cPrecipdata pd = new cPrecipdata();
                        pd.date = d;
                        pd.amt = amt;

                        pdList.Add(pd);
                    }
                }
                return pdList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    /// <summary>
    /// Class used to stored precipitation data in memory
    /// </summary>
    public class cPrecipdata
    {
        public DateTime date { get; set; }
        public decimal?  amt { get; set; }
    }
}
