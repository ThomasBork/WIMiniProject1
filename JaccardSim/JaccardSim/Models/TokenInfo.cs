using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaccardSim.Models
{
    public class TokenInfo
    {
        public Document Document { get; set; }
        public int Position { get; set; }
        public TokenInfo(Document doc, int pos)
        {
            this.Document = doc;
            this.Position = pos;
        }
    }
}
