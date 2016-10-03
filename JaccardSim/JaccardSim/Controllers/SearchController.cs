using JaccardSim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;

namespace JaccardSim.Controllers
{
    public static class SearchController
    {
        public static List<Document> TFIDFQuery(string query)
        {
            var qWords = query.Split(' ').Select(x => Program.Stemmer.stem(x)).ToList();

            var db = DBController.LoadTokenOccurances();

            List<Document> matchedDocuments = db.Documents.OrderByDescending(doc=> GetNormalizedDocumentScore(doc, db, qWords)).ToList();

            if (matchedDocuments.Count > 10)
                return matchedDocuments.Take(10).ToList();
            else
                return matchedDocuments;
        }

        private static double GetDocumentScore(Document doc, DatabaseContainer db, List<string> query)
        {
            var score = 0d;
            foreach(var term in query)
            {
                var occurance = doc.TokenOccurances.FirstOrDefault(to => to.Token.Content == term);
                if (occurance != null)
                {
                    var df = db.Tokens.First(t => t.Content == term).Frequency;
                    var docCount = db.Documents.Count;
                    var tf = occurance.Frequency;
                    var ltf = 1 + Math.Log10(tf);
                    var idf = docCount / (double)df;
                    var lidf = Math.Log10(idf);
                    score += ltf * lidf;
                }
            }
            doc.TempScore = score;
            return score;
        }

        private static double GetNormalizedDocumentScore(Document doc, DatabaseContainer db, List<string> query)
        {
            var score = 0d;

            var docLength = 0d;
            foreach (var occurance in doc.TokenOccurances)
            {
                var tf = occurance.Frequency;
                var ltf = 1 + Math.Log10(tf);
                docLength += ltf * ltf;
            }
            docLength = Math.Sqrt(docLength);

            var queryLength = 0d;
            foreach (var term in query)
            {
                var df = db.Tokens.First(t => t.Content == term).Frequency;
                var docCount = db.Documents.Count;
                var idf = docCount / (double)df;
                var lidf = Math.Log10(idf);
                queryLength += lidf * lidf;
            }
            queryLength = Math.Sqrt(queryLength);

            foreach(var term in query)
            {
                var occurance = doc.TokenOccurances.FirstOrDefault(x => x.Token.Content == term);
                if(occurance != null)
                {
                    var df = db.Tokens.First(t => t.Content == term).Frequency;
                    var docCount = db.Documents.Count;
                    var idf = docCount / (double)df;
                    var lidf = Math.Log10(idf);

                    var tf = occurance.Frequency;
                    var ltf = 1 + Math.Log10(tf);
                    score += (lidf / queryLength) * (tf / docLength);
                }
            }

            doc.TempScore = score;
            return score;
        }

        public static List<Document> BooleanQuery(string query)
        {
            query = query.Replace(" ", "");
            var db = DBController.LoadTokenOccurances(query.Replace("&", "|").Split('|').Select(x=>Program.Stemmer.stem(x)).ToList());

            var result = new List<Document>();
            var output = "";
            foreach (var or in query.Split('|'))
            {
                List<Document> matchedDocuments = db.Documents;
                foreach(var and in or.Split('&'))
                {
                    var token = Program.Stemmer.stem(and);
                    output += token + ",";
                    matchedDocuments = matchedDocuments.Where(d => d.TokenOccurances.Exists(t => t.Token.Content.Equals(token))).ToList();
                }
                result = result.Union(matchedDocuments).ToList();
            }
            Console.WriteLine(output);

            DBController.CloseConnection();
            return result;
        }
    }
}
