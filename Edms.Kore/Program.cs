using System;
using System.Data.Entity;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Edms.Kore
{
    public class Program
    {
     
        public static void Main(string[] args)
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

            Console.ReadKey();
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
