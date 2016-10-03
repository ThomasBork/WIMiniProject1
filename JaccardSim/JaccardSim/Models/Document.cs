using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaccardSim.Models
{
    public class Document
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string Uri { get; set; }
        public string ModifiedText { get; set; }
        public List<TokenOccurance> TokenOccurances { get; set; }
        public double TempScore { get; set; }
        public Document (int id, string text, string uri, string modifiedText)
        {
            this.ID = id;
            this.Text = text;
            this.Uri = uri;
            this.ModifiedText = modifiedText;
            this.TokenOccurances = new List<TokenOccurance>();
        }
    }
}
