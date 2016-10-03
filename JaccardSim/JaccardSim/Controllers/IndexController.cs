using JaccardSim.Constants;
using JaccardSim.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JaccardSim.Controllers
{
    public static class IndexController
    {
        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Uni\SW7\Web Intelligence\Miniprojekt 1\Crawler\WIMiniProject1\JaccardSim\JaccardSim\DocumentDatabase.mdf;Integrated Security=True;MultipleActiveResultSets=True;";

        public static void Index()
        {
            using (SqlConnection myConnection = new SqlConnection())
            {
                myConnection.ConnectionString = connectionString;
                myConnection.Open();
                UpdateModifiedTexts(myConnection);
                InsertTokens(myConnection);


                /*
                    
                */
            }
        }

        private static List<Document> LoadDocuments(SqlConnection myConnection)
        {
            SqlDataAdapter myDataAdapter = new SqlDataAdapter("SELECT * FROM Documents", myConnection);

            var reader = myDataAdapter.SelectCommand.ExecuteReader();

            var documents = new List<Document>();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string text = reader.GetString(1);
                string url = reader.GetString(2);
                string modifiedText = reader.GetString(3);
                var document = new Document(id, text, url, modifiedText);
                documents.Add(document);
            }
            return documents;
        }

        private static void UpdateModifiedTexts (SqlConnection myConnection)
        {
            var documents = LoadDocuments(myConnection);
            SqlDataAdapter myDataAdapter = new SqlDataAdapter("UPDATE Documents SET ModifiedText = @modText WHERE Id = @ID", myConnection);
            myDataAdapter.SelectCommand.Parameters.Add("@modText", SqlDbType.Text, int.MaxValue);
            myDataAdapter.SelectCommand.Parameters.Add("@ID", SqlDbType.Int, 0);
            myDataAdapter.SelectCommand.Prepare();

            foreach (var document in documents)
            {
                document.ModifiedText = Prettify(document.Text);
            }

            foreach(var document in documents)
            {
                myDataAdapter.SelectCommand.Parameters["@ID"].Value = document.Id;
                myDataAdapter.SelectCommand.Parameters["@modText"].Value = document.ModifiedText;
                myDataAdapter.SelectCommand.ExecuteNonQuery();
            }
        }

        private static void InsertTokens (SqlConnection myConnection)
        {
            var documents = LoadDocuments(myConnection);

            foreach (var document in documents)
            {
                Token.Tokenize(document);
            }

            var sortedTokens = Token.Tokens.Where(x => !IndexConstants.STOPWORDS.Contains(x.Content) && x.Content != "" && x.Content[0] != '[').OrderByDescending(x => x.Info.Count);

            using (SqlCommand tokenCommand = myConnection.CreateCommand())
            {
                tokenCommand.CommandText = "INSERT INTO Tokens (Word, Frequency) VALUES (@word, @frequency)";
                tokenCommand.Parameters.Add("@word", SqlDbType.Text, 50);
                tokenCommand.Parameters.Add("@frequency", SqlDbType.Int, 0);
                tokenCommand.Prepare();

                using (SqlCommand tokenInfoCommand = myConnection.CreateCommand())
                {
                    tokenInfoCommand.CommandText = "INSERT INTO TokenOccurances (TokenID, DocID, Frequency) VALUES (@tokenID, @docID, @frequency)";
                    tokenInfoCommand.Parameters.Add("@word", SqlDbType.Text, 50);
                    tokenInfoCommand.Parameters.Add("@url", SqlDbType.Text, 50);
                    tokenInfoCommand.Parameters.Add("@tokenID", SqlDbType.Int, 0);
                    tokenInfoCommand.Parameters.Add("@docID", SqlDbType.Int, 0);
                    tokenInfoCommand.Parameters.Add("@frequency", SqlDbType.Int, 0);
                    tokenInfoCommand.Prepare();

                    using (SqlCommand tokenIdCommand = myConnection.CreateCommand())
                    {
                        tokenIdCommand.CommandText = "SELECT TOP 1 Id FROM Tokens WHERE Word LIKE @word";
                        tokenIdCommand.Parameters.Add("@word", SqlDbType.Text, 50);
                        tokenIdCommand.Prepare();

                        foreach (var token in sortedTokens)
                        {
                            tokenCommand.Parameters["@word"].Value = token.Content;
                            tokenCommand.Parameters["@frequency"].Value = token.Info.Count;
                            tokenCommand.ExecuteNonQuery();

                            tokenIdCommand.Parameters["@word"].Value = token.Content;
                            var reader = tokenIdCommand.ExecuteReader();
                            var tokenId = -1;
                            while (reader.Read())
                            {
                                tokenId = reader.GetInt32(0);
                            }
                            reader.Close();

                            foreach (var info in token.Info)
                            {
                                tokenInfoCommand.Parameters["@word"].Value = token.Content;
                                tokenInfoCommand.Parameters["@url"].Value = info.Document.Uri;
                                tokenInfoCommand.Parameters["@tokenID"].Value = tokenId;
                                tokenInfoCommand.Parameters["@docID"].Value = info.Document.Id;
                                tokenInfoCommand.Parameters["@frequency"].Value = info.Frequency;
                                tokenInfoCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        private static string Prettify(string text)
        {
            var regex = new Regex( "(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var sb = new StringBuilder();
            bool isSpace = false;
            text = regex.Replace(text, "");
            text = text.ToLower();

            bool textFound = false;
            for(int i = 0; i<text.Length; i++)
            {
                switch (text[i])
                {
                    case '>':
                        textFound = true;
                        break;
                    case '<':
                        textFound = false;
                        break;
                    case ' ':
                    case '\n':
                    case '\t':
                    case '\r':
                    case '.':
                    case ',':
                    case '{':
                    case '}':
                    case '=':
                    case ':':
                    case '|':
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                    case '&':
                    case '%':
                    case '/':
                    case '\\':
                    case '-':
                    case '#':
                    case '_':
                        if (textFound && !isSpace)
                        {
                            isSpace = true;
                        }
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                    case 'æ':
                    case 'ø':
                    case 'å':
                        if (textFound)
                        {
                            if (isSpace)
                            {
                                isSpace = false;
                                sb.Append(' ');
                            }
                            sb.Append(text[i]);
                        }
                        break;
                    default:
                        if (textFound && !isSpace)
                        {
                            isSpace = true;
                        }
                        break;
                }
            }
            var newText = sb.ToString();
            return newText;
        }
    }
}
