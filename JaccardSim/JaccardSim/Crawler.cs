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
using JaccardSim.Models;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data;
using JaccardSim.Controllers;

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
        private bool shouldStop = false;

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
            
            EnqueueAddressOnce(startPage);

            crawlerThread = new Thread(Crawl);
        }

        private void EnqueueAddressOnce(string address)
        {
            Monitor.Enter(urlsToCrawlMutex);
            if (address.StartsWith("http://"+domain) &&
                !urlsToCrawl.Contains(address) &&
                !results.ContainsKey(address) &&
                disallowed.TrueForAll(x => !address.StartsWith("http://"+domain+x)))
            {
                urlsToCrawl.Enqueue(address);
            }
            Monitor.Exit(urlsToCrawlMutex);
        }

        private void Crawl()
        {
            using (WebClient client = new WebClient())
            {
                while(true)
                {
                    if (urlsToCrawl.Count < 1 || shouldStop)
                    {
                        ShareResults();
                        return;
                    }

                    var url = urlsToCrawl.Peek();

                    try
                    {
                        var html = client.DownloadString(url);
                        results.Add(url, html);
                        urlsToCrawl.Dequeue();

                        var htmlDocument = new HtmlDocument();

                        htmlDocument.LoadHtml(html);
                        
                        using (SqlConnection myConnection = new SqlConnection())
                        {
                            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Uni\SW7\Web Intelligence\Miniprojekt 1\Crawler\WIMiniProject1\JaccardSim\JaccardSim\DocumentDatabase.mdf;Integrated Security=True;";

                            myConnection.ConnectionString = connectionString;
                            myConnection.Open();
                            
                            SqlDataAdapter myDataAdapter = new SqlDataAdapter("INSERT INTO Documents (Text, Url) VALUES (" + "@html" + ", " + "@url" + ")", myConnection);

                            myDataAdapter.SelectCommand.Parameters.AddWithValue("@html", html);
                            myDataAdapter.SelectCommand.Parameters.AddWithValue("@url", url);
                            myDataAdapter.SelectCommand.ExecuteNonQuery();
                        }

                        var hrefs = htmlDocument.DocumentNode.SelectNodes("//a/@href");
                        if (hrefs == null)
                        {
                            continue;
                        }
                        foreach (HtmlNode node in hrefs)
                        {
                            string href = node.GetAttributeValue("href", "#");
                            if (href.Length < 1 || href[0] == '#' || href.StartsWith("mailto:"))
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

                            EnqueueAddressOnce(urlToAdd);

                            if (!urlToAdd.StartsWith("http://" + domain))
                            {
                                CrawlerController.NewDomains.Enqueue(urlToAdd);
                            }
                        }

                        Monitor.Enter(CrawlerController.CountMutex);
                        CrawlerController.Count++;
                        Monitor.Exit(CrawlerController.CountMutex);
                    }
                    catch (WebException)
                    {
                        urlsToCrawl.Dequeue();
                        //not much to do here
                        //Console.WriteLine("Exception found at " + url + " with the message " + e);
                    }
                }
            }
        }

        private void ShareResults()
        {
            foreach (KeyValuePair<string, string> kvp in results)
            {
                CrawlerController.CombinedResults.TryAdd(kvp.Key, kvp.Value);
            }
        }

        public void Start()
        {
            crawlerThread.Start();
        }

        public void Stop()
        {
            shouldStop = true;
        }

        public void AddHref(string href)
        {
            EnqueueAddressOnce(href);
            if (!crawlerThread.IsAlive)
            {
                shouldStop = false;
                crawlerThread = new Thread(Crawl);
                crawlerThread.Start();
            }
        }
    }
}
