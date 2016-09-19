using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.XPath;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using System.Threading;

namespace WebCrawler
{
    public class Crawler
    {
        private static long idCount = 0;
        private readonly long id;

        private readonly string startPage;
        private readonly long pageCap;
        public readonly string domain;
        private readonly List<string> disallowed;

        private Thread crawlerThread;

        private Dictionary<string, string> results = new Dictionary<string, string>();

        private object urlsToCrawlMutex = new object();
        private Queue<string> urlsToCrawl = new Queue<string>();

        public Crawler(string startPage, long pageCap)
        {
            id = idCount++;
            this.startPage = startPage;
            this.pageCap = pageCap;
            this.domain = new Uri(startPage).Host;
            this.disallowed = RobotsParser.Parse("http://" + domain + "/robots.txt");
            if (ShouldAddressBeEnqueued(startPage))
            {
                urlsToCrawl.Enqueue(startPage);
            }

            crawlerThread = new Thread(Crawl);
        }

        private bool ShouldAddressBeEnqueued(string address)
        {
            return (address.StartsWith("http://"+domain) &&
                !urlsToCrawl.Contains(address) &&
                !results.ContainsKey(address) &&
                disallowed.TrueForAll(x => !address.StartsWith("http://"+domain+x)));
        }

        private void Crawl()
        {
            using (WebClient client = new WebClient())
            {
                for (int i = 0; i < pageCap; i++)
                {
                    if (urlsToCrawl.Count < 1)
                    {
                        return;
                    }

                    var url = urlsToCrawl.Dequeue();

                    try
                    {
                        var html = client.DownloadString(url);
                        results.Add(url, html);

                        var htmlDocument = new HtmlDocument();

                        htmlDocument.LoadHtml(html);

                        var hrefs = htmlDocument.DocumentNode.SelectNodes("//a/@href");
                        if (hrefs == null)
                        {
                            continue;
                        }
                        foreach (HtmlNode node in hrefs)
                        {
                            string href = node.GetAttributeValue("href", "#");
                            if (href.Length < 1 || href[0] == '#')
                            {
                                continue;
                            }

                            string urlToAdd;

                            if (href[0] == '/' || href[0] == '?')
                            {
                                urlToAdd = url.Remove(url.Length - 1) + href;
                            }
                            else
                            {
                                urlToAdd = href;
                            }

                            Monitor.Enter(urlsToCrawlMutex);
                            if (ShouldAddressBeEnqueued(urlToAdd))
                            {
                                urlsToCrawl.Enqueue(urlToAdd);
                            }
                            else if (!urlToAdd.StartsWith("http://" + domain))
                            {
                                Program.newDomains.Enqueue(urlToAdd);
                            }
                            Monitor.Exit(urlsToCrawlMutex);

                        }

                        Monitor.Enter(Program.countMutex);
                        Program.count++;
                        Monitor.Exit(Program.countMutex);

                        Console.WriteLine(Program.count + ": Crawler " + id + "/" + idCount + " crawled page " + i + ": " + url + " \nCurrent queuelength: " + urlsToCrawl.Count);

                    }
                    catch (WebException)
                    {
                        //not much to do here
                        //Console.WriteLine("Exception found at " + url + " with the message " + e);
                    }
                }
                this.Stop();
            }
        }

        public void Start()
        {
            crawlerThread.Start();
        }

        public void Stop()
        {
            crawlerThread.Abort();

            foreach (KeyValuePair<string, string> kvp in results)
            {
                while (!Program.combinedResults.TryAdd(kvp.Key, kvp.Value)) ;
            }
        }

        public void AddHref(string href)
        {
            Monitor.Enter(urlsToCrawlMutex);
            if (ShouldAddressBeEnqueued(href))
            {
                urlsToCrawl.Enqueue(href);
            }
            Monitor.Exit(urlsToCrawlMutex);
        }
    }
}
