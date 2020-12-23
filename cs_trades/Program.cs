using System;
using System.Buffers;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

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
            int VOLUME,
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

        long m_TRADENO { get; }
        string m_TRADETIME { get; }
        string m_SECBOARD { get; }
        string m_SECCODE { get; }
        double m_PRICE { get; }
        int m_VOLUME { get; }
        double m_ACCRUEDINT { get; }
        double m_YIELD { get; }
        double m_VALUE { get; }
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
                        int VOLUME = int.Parse (parts[i++]);
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

        static void Main (string[] args)
        {
            string path = "C:\\Users\\User\\Desktop\\short.txt";
            List<TradeInfo> trades = get_all_trades (path);

            foreach (var tr in trades)
                tr.print ();
        }
    }
}
