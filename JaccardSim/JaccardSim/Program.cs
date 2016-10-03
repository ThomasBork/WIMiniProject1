using JaccardSim.Controllers;
using JaccardSim.Libs;
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
        static readonly long indexSize = 1000;

        public static object countMutex = new object();
        public static int count = 0;

        public static ConcurrentDictionary<string,string> combinedResults = new ConcurrentDictionary<string,string>();
        public static ConcurrentQueue<string> newDomains = new ConcurrentQueue<string>();
        public static Dictionary<string, Crawler> crawlers = new Dictionary<string, Crawler>();


        public static Porter2 Stemmer { get; set; }

        [STAThread]
        static void Main()
        {
            Program.Stemmer = new Porter2();
            StartForm();
        }
        
        static void StartForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new JaccardSim.MainForm());
        }

        static void Index()
        {
            IndexController.Index();
        }

        static void Crawl()
        {
            Thread crawlerManagerThread = new Thread(newDomainListening);
            crawlerManagerThread.Priority = ThreadPriority.Highest;
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
                        var dictionaryKey = new Uri(uri).Host;
                        if (crawlers.ContainsKey(dictionaryKey))
                        {
                            Crawler existingCrawler = crawlers[dictionaryKey];
                            existingCrawler.AddHref(uri);
                        }
                        else
                        {
                            Crawler newCrawler = new Crawler(uri, indexSize);
                            crawlers.Add(dictionaryKey, newCrawler);
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
