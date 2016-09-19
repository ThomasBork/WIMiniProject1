using JaccardSim.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;

namespace JaccardSim.Models
{
    public class Token
    {
        private static List<Token> _tokens = new List<Token>();
        public static List<Token> Tokens {
            get { return _tokens; }
            set {  _tokens = value; }
        }

        public string Content { get; set; }
        public List<TokenInfo> Info { get; }

        public Token(string content)
        {
            this.Content = content;
            this.Info = new List<TokenInfo>();
        }

        public static void Tokenize (string text, Document doc)
        {
            var wordArray = text.Split(' ');
            for(int i = 0; i < wordArray.Length; i++)
            {
                var word = Program.Stemmer.stem(wordArray[i]);
                var token = Tokens.FirstOrDefault(x => x.Content == word);
                if (token != null)
                {
                    token = new Token(word);
                    Tokens.Add(token);
                }
                token.Info.Add(new TokenInfo(doc, i));
            }
        }
    }
}
