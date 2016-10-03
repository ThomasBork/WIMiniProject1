using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler;

namespace JaccardSim.Controllers
{
    public static class CrawlerController
    {
        private static string start = "http://inktank.fi/the-17-most-wonderfully-weird-websites-on-the-internet/";
        private static readonly long indexSize = 1000;

        private static object _countMutex = new object();
        public static object CountMutex { get { return _countMutex; } }

        public static int Count { get; set; }

        private static ConcurrentDictionary<string, string> _combinedResults = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, string> CombinedResults { get { return _combinedResults; } }

        private static ConcurrentQueue<string> _newDomains = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> NewDomains { get { return _newDomains; } }

        private static Dictionary<string, Crawler> _crawlers = new Dictionary<string, Crawler>();
        public static Dictionary<string, Crawler> Crawlers { get { return _crawlers; } }

        private static Thread crawlerManagerThread;

        public static void Start()
        {
            if (crawlerManagerThread == null || !crawlerManagerThread.IsAlive)
            {
                crawlerManagerThread = new Thread(NewDomainListening);
                crawlerManagerThread.Priority = ThreadPriority.Highest;
                crawlerManagerThread.Start();
                NewDomains.Enqueue(start);
            }
        }

        public static void Stop()
        {
            if (crawlerManagerThread != null && crawlerManagerThread.IsAlive)
            {
                crawlerManagerThread.Abort();
                foreach(var kvp in Crawlers)
                {
                    kvp.Value.Stop();
                }
            }
        }

        public static void NewDomainListening()
        {
            while (true)
            {
                if (!NewDomains.IsEmpty && Count < indexSize)
                {
                    string uri;
                    if (NewDomains.TryDequeue(out uri))
                    {
                        var dictionaryKey = new Uri(uri).Host;
                        if (Crawlers.ContainsKey(dictionaryKey))
                        {
                            Crawler existingCrawler = Crawlers[dictionaryKey];
                            existingCrawler.AddHref(uri);
                        }
                        else
                        {
                            Crawler newCrawler = new Crawler(uri, indexSize);
                            Crawlers.Add(dictionaryKey, newCrawler);
                            newCrawler.Start();
                        }
                    }
                }
                if (Count >= indexSize)
                {
                    foreach (KeyValuePair<string, Crawler> kvp in Crawlers)
                    {
                        kvp.Value.Stop();
                    }
                }
            }
        }
    }
}
