using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Edms.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            string root_data_directory = @"C:\Users\germg\code\data\gutenberg";

            var data_directories = Directory.GetDirectories(root_data_directory);

            foreach(var directory in Directory.GetDirectories(root_data_directory))
            {
                Console.WriteLine($"DIRECTORY: {directory}");
                foreach (var file in Directory.GetFiles(directory))
                {
                    Console.WriteLine($"Working on file: {file}");
                    var fileContents = File.ReadAllText(file);
                    Console.WriteLine($"Calculating md5 sum for {file}");
                    Console.WriteLine(CalculateMD5Hash(fileContents));
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
