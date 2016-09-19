using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;

namespace WebCrawler
{
    public static class RobotsParser
    {
        public const string USER_AGENT_NAME = "GoodBot";

        public static List<string> Parse(string uri)
        {
            string robots = "placeholder";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, USER_AGENT_NAME);
                try
                {
                    robots = client.DownloadString(uri);
                }
                catch (Exception) { }
            }
            if (robots == "placeholder")
            {
                return new List<string>();
            }

            robots = robots.Replace("\r","");
            string[] lines = robots.Split('\n');
            bool care = false;
            List<string> disallowed = new List<string>();

            foreach (string line in lines)
            {
                string[] elements = line.Split(' ');
                if (elements.Length < 1)
                {
                    continue;
                }
                if (elements[0].ToLower().Equals("user-agent:"))
                {
                    if (elements[1].Equals("*") || elements[1].Equals(USER_AGENT_NAME)) 
                    {
                        care = true;
                    }
                    else
                    {
                        care = false;
                    }
                }else if (elements[0].ToLower().Equals("disallow:") && care)
                {
                    if (elements.Length > 1)
                    {
                        disallowed.Add(elements[1]);
                    }
                    
                }
            }
            return disallowed;
        }
    }
}
