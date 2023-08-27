namespace DocumentProcessor.Data.Commands
{
    using DocumentProcessor.Data.Models;

    public class DocumentProcessResponse
    {
        public List<Document> Documents { get; set; } = new List<Document>();
        public int LineCount { get; set; }
        public int CharCount { get; set; }
        public decimal Sum { get; set; }
        public int XCount { get; set; }
        public string ProductsWithMaxNetValue { get; set; } = string.Empty;
    }
}
