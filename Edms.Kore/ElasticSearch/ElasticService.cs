using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edms.Kore.Model;

namespace Edms.Kore.ElasticSearch
{
    public class ElasticService
    {
        private readonly ElasticClient _elastic;

        public ElasticService(ElasticClient elastic)
        {
            _elastic = elastic;
        }

        public void Index(Document postModel)
        {
            var post = new OCR_DocumentType
            {
                FileHash = postModel.FileHash,
                FileName = postModel.FileName,
                Content = postModel.Content,
            };

            _elastic.Index(post);
        }

        public void Delete(int id)
        {
            _elastic.Delete(id);
        }

        public List<Document> Search(string terms)
        {
            if (string.IsNullOrEmpty(terms))
            {
                return new List<Document>();
            }

            var response = _elastic.Search(terms);

            string dslQuery = Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes);

            var result = response.Hits
                .Select(p => new Document
                {
                    FileHash = p.Source.FileHash,
                    FileName = p.Source.FileName,
                    Content = p.Source.Content,
                })
                .ToList();

            return result;
        }
    }
}