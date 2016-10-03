using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaccardSim.Models
{
    public class TokenOccurance
    {
        public Token Token { get; set; }
        public Document Document { get; set; }
        public int Frequency { get; set; }

        public TokenOccurance(Document doc)
        {
            this.Document = doc;
        }

        public TokenOccurance(Token token, Document doc, int frequency)
        {
            this.Token = token;
            this.Document = doc;
            this.Frequency = frequency;
        }
    }
}
