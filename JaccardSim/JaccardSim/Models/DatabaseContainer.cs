using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaccardSim.Models
{
    public class DatabaseContainer
    {
        public List<Document> Documents { get; set; }
        public List<Token> Tokens { get; set; }
        public List<TokenOccurance> TokenOccurances { get; set; }
        public DatabaseContainer (List<Document> documents, List<Token> tokens, List<TokenOccurance> tokenOccurances)
        {
            this.Documents = documents;
            this.TokenOccurances = tokenOccurances;
            this.Tokens = tokens;
        }
    }
}
