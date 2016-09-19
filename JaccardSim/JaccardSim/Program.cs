using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebCrawler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        static string start = "http://inktank.fi/the-17-most-wonderfully-weird-websites-on-the-internet/";
        static readonly long indexSize = 100;

        public static object countMutex = new object();
        public static int count = 0;

        public static ConcurrentDictionary<string,string> combinedResults = new ConcurrentDictionary<string,string>();
        public static ConcurrentQueue<string> newDomains = new ConcurrentQueue<string>();
        public static ConcurrentDictionary<string, Crawler> crawlers = new ConcurrentDictionary<string, Crawler>();

        [STAThread]
        static void Main()
        {
            Thread crawlerManagerThread = new Thread(newDomainListening);
            crawlerManagerThread.Start();
            newDomains.Enqueue(start);
        }

        static void newDomainListening()
        {
            while(true)
            {
                if (!newDomains.IsEmpty && count < indexSize)
                {
                    string uri;
                    if (newDomains.TryDequeue(out uri))
                    {
                        if (crawlers.ContainsKey(new Uri(uri).Host))
                        {
                            Crawler existingCrawler;
                            while (!crawlers.TryGetValue(uri, out existingCrawler)) ;
                            existingCrawler.AddHref(uri);
                        }
                        else
                        {
                            Crawler newCrawler = new Crawler(uri, indexSize);
                            newCrawler.Start();
                        }
                    }
                }
                if (count >= indexSize)
                {
                    foreach (KeyValuePair<string, Crawler> kvp in crawlers)
                    {
                        kvp.Value.Stop();
                    }
                }
            }
        }
    }
}
