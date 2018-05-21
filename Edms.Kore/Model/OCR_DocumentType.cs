using Nest;

namespace Edms.Kore.Model
{
    [ElasticsearchType(Name = "ocr_document")]
    public class OCR_DocumentType
    {
        [Text] public string FileHash { get; set; }
        [Text] public string Folder { get; set; }
        [Text] public string FileName { get; set; }
        [Text] public string Content { get; set; }
    }
}
