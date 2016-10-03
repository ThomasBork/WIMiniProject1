using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaccardSim.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Uri { get; set; }
        public string ModifiedText { get; set; }
        public Document (int id, string text, string uri, string modifiedText)
        {
            this.Id = id;
            this.Text = text;
            this.Uri = uri;
            this.ModifiedText = modifiedText;
        }
    }
}
