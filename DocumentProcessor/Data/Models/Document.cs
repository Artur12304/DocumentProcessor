namespace DocumentProcessor.Data.Models
{
    public class Document
    {
        public DocumentHeader Header { get; set; }
        public List<DocumentItem> Items { get; set; } = new List<DocumentItem>();
        public string Description { get; set; } = String.Empty;
    }
}