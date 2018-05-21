using Edms.Kore.ElasticSearch;
using Nest;
using System;
using System.Data.Entity;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Edms.Kore
{
    [ElasticsearchType(Name="ocr_document")]
    public class OCR_DocumentType
    {
        [Text] public string FileHash { get; set; }
        [Text] public string Folder { get; set; }
        [Text] public string FileName { get; set; }
        [Text] public string Content { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            //PopulateDatabase();
            ElasticSearch.ElasticClient.Initialize();
            var ec = new ElasticSearch.ElasticClient();
            var elastic_service = new ElasticService(ec);

            //var db_context = new ElasticSearchProxyEntities();

            //var k = db_context.Documents;
            //foreach(var k1 in k)
            //{
            //    elastic_service.Index(k1);
            //}

            var k = elastic_service.Search("blue");


            Console.ReadKey();
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


        //public static void Initialize()
        //{
        //    var _indexName = "cfw_documents";

        //    var _elastic = GetClient();

        //    _elastic.CreateIndex(
        //        _indexName, 
        //        i => i
        //            .Settings(s => s
        //                .Setting("number_of_shards", 1)
        //                .Setting("number_of_replicas", 0)));


        //    _elastic.Map<OCR_Document>(x => x
        //        .Index(_indexName)
        //        .AutoMap());
        //}


        //public static IElasticClient GetClient()
        //{
        //    var urlString = new Uri("http://localhost:9200");
        //    var settings = new ConnectionSettings(urlString).DisableDirectStreaming();

        //    return new Nest.ElasticClient(settings);
        //}
    }
}
