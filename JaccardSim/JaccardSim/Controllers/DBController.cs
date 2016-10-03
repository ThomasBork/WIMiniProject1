using JaccardSim.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaccardSim.Controllers
{
    public static class DBController
    {
        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Uni\SW7\Web Intelligence\Miniprojekt 1\Crawler\WIMiniProject1\JaccardSim\JaccardSim\DocumentDatabase.mdf;Integrated Security=True;MultipleActiveResultSets=True;";
        private static SqlConnection sqlConnection = null;
        public static List<Document> LoadDocuments()
        {
            GetConnection();
            SqlDataAdapter myDataAdapter = new SqlDataAdapter("SELECT * FROM Documents", sqlConnection);

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

        public static List<Token> LoadTokens(List<string> terms = null)
        {
            List<Token> tokens = new List<Token>();
            var query = "SELECT * FROM Tokens T ";
            if(terms != null)
            {
                query += "WHERE ";
                for(int i = 0; i<terms.Count; i++)
                {
                    //SQL Injection DANGER!!
                    query += "T.Word = '" + terms[i] + "' ";
                    if(i != terms.Count - 1)
                    {
                        query += "OR ";
                    }  
                }
            }
            GetConnection();
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(query, sqlConnection);

            var reader = myDataAdapter.SelectCommand.ExecuteReader();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var word = reader.GetString(1);
                var frequency = reader.GetInt32(2);
                var token = new Token(id, word, frequency);
                tokens.Add(token);
            }
            CloseConnection();

            return tokens;
        }

        public static DatabaseContainer LoadTokenOccurances(List<string> terms = null)
        {
            var tokens = LoadTokens(terms);
            var documents = LoadDocuments();
            var tokenOccurances = new List<TokenOccurance>();

            var query = "SELECT * FROM TokenOccurances";

            GetConnection();
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(query, sqlConnection);

            var reader = myDataAdapter.SelectCommand.ExecuteReader();

            while (reader.Read())
            {
                var tokenID = reader.GetInt32(0);
                var documentID = reader.GetInt32(1);
                var frequency = reader.GetInt32(2);
                var token = tokens.FirstOrDefault(x => x.ID == tokenID);
                if (token != null)
                {
                    var document = documents.First(x => x.ID == documentID);
                    var tokenOccurance = new TokenOccurance(token, document, frequency);
                    tokenOccurances.Add(tokenOccurance);
                    document.TokenOccurances.Add(tokenOccurance);
                    token.TokenOccurances.Add(tokenOccurance);
                }
            }
            CloseConnection();

            documents = documents.Where(doc => doc.TokenOccurances.Count > 0).ToList();

            return new DatabaseContainer(documents, tokens, tokenOccurances);
        }

        public static SqlConnection GetConnection()
        {
            if(sqlConnection == null)
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
            }
            return sqlConnection;
        }
        public static void CloseConnection ()
        {
            sqlConnection?.Close();
            sqlConnection = null;
        }
    }
}
