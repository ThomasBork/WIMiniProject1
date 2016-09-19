using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaccardSim.Models
{
    public class Document
    {
        public string Text { get; set; }
        public string Uri { get; set; }
        public Document (string text, string uri)
        {
            this.Text = text;
            this.Uri = uri;
        }
    }
}
