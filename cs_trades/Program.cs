using System;
using System.Buffers;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Linq;

namespace cs_trades
{
    struct TradeInfo
    {
        public TradeInfo (
            long TRADENO, 
            string TRADETIME, 
            string SECBOARD,
            string SECCODE,
            double PRICE,
            long VOLUME,
            double ACCRUEDINT,
            double YIELD,
            double VALUE)
        {
            m_TRADENO = TRADENO;
            m_TRADETIME = TRADETIME;
            m_SECBOARD = SECBOARD;
            m_SECCODE = SECCODE;
            m_PRICE = PRICE;
            m_VOLUME = VOLUME;
            m_ACCRUEDINT = ACCRUEDINT;
            m_YIELD = YIELD;
            m_VALUE = VALUE;
        }

        public void print ()
        {
            foreach (var field in typeof (TradeInfo).GetFields (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                Console.WriteLine ("{0} = {1}", field.Name, field.GetValue (this));
            Console.WriteLine ();
        }

        public long m_TRADENO { get; }
        public string m_TRADETIME { get; }
        public string m_SECBOARD { get; }
        public string m_SECCODE { get; }
        public double m_PRICE { get; }
        public long m_VOLUME { get; }
        public double m_ACCRUEDINT { get; }
        public double m_YIELD { get; }
        public double m_VALUE { get; }
    }

    class Program
    {
        static List<TradeInfo> get_all_trades (string path)
        {
            try
            {
                List<TradeInfo> result = new List<TradeInfo> ();
                using (StreamReader sr = new StreamReader (path))
                {
                    string line;
                    while ((line = sr.ReadLine ()) != null)
                    {
                        string[] parts = line.Split ("\t");

                        if (parts[0] == "TRADENO")
                            continue;

                        int i = 0;
                        long TRADENO = long.Parse (parts[i++].TrimStart ('0'));
                        string TRADETIME = parts[i++];
                        string SECBOARD = parts[i++];
                        string SECCODE = parts[i++];
                        double PRICE = double.Parse (parts[i++], CultureInfo.InvariantCulture);
                        long VOLUME = long.Parse (parts[i++]);
                        double ACCRUEDINT = double.Parse (parts[i++], CultureInfo.InvariantCulture);
                        double YIELD = double.Parse (parts[i++], CultureInfo.InvariantCulture);
                        double VALUE = double.Parse (parts[i++], CultureInfo.InvariantCulture);

                        result.Add (new TradeInfo (TRADENO, TRADETIME, SECBOARD, SECCODE, PRICE, VOLUME, ACCRUEDINT, YIELD, VALUE));
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine ("The file could not be read:");
                Console.WriteLine (e.Message);
                return new List<TradeInfo> ();
            }
        }

        static void describe_group (IGrouping<string, TradeInfo> sg)
        {
            double percentchange = Math.Round ((sg.Last ().m_PRICE - sg.First ().m_PRICE) / sg.First ().m_PRICE * 100, 2);
            double fullsum = sg.Sum (s => { return s.m_VALUE; });

            Console.WriteLine ("Seccode: {0}", sg.First ().m_SECCODE);
            Console.WriteLine ("Price change: {0}%", percentchange);
            Console.WriteLine ("Total sum: {0}", fullsum);

            Console.WriteLine ();
        }

        static void Main (string[] args)
        {
            string path = "C:\\Users\\User\\Desktop\\trades.txt";
            List<TradeInfo> trades = get_all_trades (path);

            var query1 = trades.Where (s => (s.m_SECBOARD == "TQBR" || s.m_SECBOARD == "FQBR"));

            var query2 = from s in query1
                         group s by s.m_SECCODE into sg
                         orderby (sg.Last ().m_PRICE - sg.First ().m_PRICE) / sg.First ().m_PRICE
                         select sg;

            var top10worst = query2.Take (10);
            var top10best = query2.Reverse ().Take (10);

            Console.WriteLine ("Top 10 best:");
            foreach (var sg in top10best)
                describe_group (sg);

            Console.WriteLine ("Top 10 worst:");
            foreach (var sg in top10worst)
                describe_group (sg);
        }
    }
}
