using JaccardSim.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler;

namespace JaccardSim.Models
{
    public class Token
    {
        private static object tokenizeLock = new object();
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

        public static void Tokenize (Document doc)
        {
            var wordArray = doc.ModifiedText.Split(' ');
            for(int i = 0; i < wordArray.Length; i++)
            {
                string word = wordArray[i];
                try { word = Program.Stemmer.stem(wordArray[i]); }
                catch (Exception e) {  }

                var token = Tokens.FirstOrDefault(x => x.Content == word);
                if (token == null)
                {
                    token = new Token(word);
                    Tokens.Add(token);
                }
                var tokenInfo = token.Info.FirstOrDefault(x => x.Document == doc);
                if(tokenInfo == null)
                {
                    tokenInfo = new TokenInfo(doc);
                    token.Info.Add(tokenInfo);
                }
                tokenInfo.Frequency++;
            }
        }
    }
}
