using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Edms.Kore.ElasticSearch;

namespace Edms.Kore
{


    public class Program
    {
        public static ElasticService elastic_service { get; private set; }
        public static ElasticSearchProxyEntities db_context = new ElasticSearchProxyEntities();

        public static void Main(string[] args)
        {
            PopulateDatabase();

            InitializeElasticService();

            IndexElasticSearch();

            var searchResult = elastic_service.Search("blue");

            foreach (var result in searchResult)
            {
                Console.WriteLine($"----{result.FileName}");
                var found_location = result.Content.IndexOf("blue");
                do
                {
                    Console.WriteLine($"  Exists @ : {found_location} : {result.Content.Substring(found_location - 10, 20)}");
                    found_location = result.Content.IndexOf("blue", found_location+1);
                }
                while (found_location > 0);
            }

            Console.ReadKey();
        }

        private static void InitializeElasticService()
        {
            ElasticSearch.ElasticClient.Initialize();
            elastic_service = new ElasticService(new ElasticSearch.ElasticClient());
        }

        private static void IndexElasticSearch()
        {
            foreach (var doc in db_context.Documents)
            {
                elastic_service.Index(doc);
            }
        }

        private static void PopulateDatabase()
        {
            var db_context = new ElasticSearchProxyEntities();

            string root_data_directory = @"C:\Users\germg\code\data\gutenberg";

            foreach (var directory in Directory.GetDirectories(root_data_directory))
            {
                Console.WriteLine($"\nDIRECTORY: {directory}");
                foreach (var file in Directory.GetFiles(directory))
                {
                    Console.WriteLine($"Working on file: {file}");
                    var fileContents = File.ReadAllText(file);
                    var md5 = CalculateMD5Hash(fileContents);

                    if (db_context.Documents.Find(md5) == null)
                    {
                        var d = new Document
                        {
                            FileHash = md5,
                            FileName = file,
                            Folder = directory,
                            Content = fileContents
                        };
                        db_context.Documents.Add(d);
                        db_context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine($"\nFile: {file} found.");
                    }
                }
            }
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
