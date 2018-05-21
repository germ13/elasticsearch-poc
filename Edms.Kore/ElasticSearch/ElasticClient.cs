using System;
using Nest;
using Edms.Kore.Model;

namespace Edms.Kore.ElasticSearch
{
    public class ElasticClient
    {
        private const string _indexName = "cfw_documents";
        public static IElasticClient _elastic { get; set; }

        public static void Initialize()
        {
            _elastic = GetClient();

            // Creating the index
            _elastic.CreateIndex(_indexName, i => i
                .Settings(s => s
                    .Setting("number_of_shards", 1)
                    .Setting("number_of_replicas", 0)));

            // Creating the types
            _elastic.Map<OCR_DocumentType>(x => x
                .Index(_indexName)
                .AutoMap());
        }

        private static IElasticClient GetClient()
        {
            var urlString = new Uri("http://localhost:9200"); // ES default url
            var settings = new ConnectionSettings(urlString).DisableDirectStreaming();

            return new Nest.ElasticClient(settings);
        }

        public void Delete(int id)
        {
            _elastic.Delete<OCR_DocumentType>(id, i => i.Index(_indexName));
        }

        public void Index(OCR_DocumentType document)
        {
            _elastic.Index(document, i => i.Id(document.FileHash.ToString())
                .Index(_indexName)
                .Type<OCR_DocumentType>());
        }

        public ISearchResponse<OCR_DocumentType> Search(string terms)
        {
            var result = _elastic.Search<OCR_DocumentType>(s => s
                .Index(_indexName)
                .Type<OCR_DocumentType>()
                .Query(q => BuildQuery(terms)));

            return result;
        }

        private QueryContainer BuildQuery(string terms)
        {
            QueryContainer query = Query<OCR_DocumentType>.MultiMatch(mm => mm
                    .Query(terms)
                    .Type(TextQueryType.MostFields)
                    .Fields(f => f
                        .Field(ff => ff.Content, boost: 3)
                        //.Field(ff => ff.FileName, boost: 2)
                        //.Field(ff => ff.Folder, boost: 1)
                    ));

            return query;
        }
    }
}
