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
        public int Frequency { get; set; }
        public TokenInfo(Document doc)
        {
            this.Document = doc;
            Frequency = 0;
        }
    }
}
