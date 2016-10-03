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
        private static List<Token> _tokens = new List<Token>();
        public static List<Token> Tokens {
            get { return _tokens; }
            set {  _tokens = value; }
        }

        public string Word { get; set; }
        public List<TokenOccurance> TokenOccurances { get; set; }

        public int ID { get; set; }
        public int Frequency { get; set; }

        public Token(string content)
        {
            this.Word = content;
            this.TokenOccurances = new List<TokenOccurance>();
        }

        public Token(int id, string word, int frequency)
        {
            this.ID = id;
            this.Word = word;
            this.Frequency = frequency;
            this.TokenOccurances = new List<TokenOccurance>();
        }

        public static void Tokenize (Document doc)
        {
            var wordArray = doc.ModifiedText.Split(' ');
            for(int i = 0; i < wordArray.Length; i++)
            {
                string word = wordArray[i];
                try { word = Program.Stemmer.stem(wordArray[i]); }
                catch (Exception e) {  }

                var token = Tokens.FirstOrDefault(x => x.Word == word);
                if (token == null)
                {
                    token = new Token(word);
                    Tokens.Add(token);
                }
                var tokenInfo = token.TokenOccurances.FirstOrDefault(x => x.Document == doc);
                if(tokenInfo == null)
                {
                    tokenInfo = new TokenOccurance(doc);
                    token.TokenOccurances.Add(tokenInfo);
                }
                tokenInfo.Frequency++;
            }
        }
    }
}
