namespace DocumentProcessor.Data.Models
{
    public class DocumentHeader
    {
        public string CodeBA { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public int DocumentDayNumber { get; set; }
        public string ContractorCode { get; set; } = string.Empty;
        public string ContractorName { get; set; } = string.Empty;
        public string ExternalDocumentNumber { get; set; } = string.Empty;
        public DateTime ExternalDocumentDate { get; set; }
        public decimal NetAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public string Field1 { get; set; } = string.Empty;
        public string Field2 { get; set; } = string.Empty;
        public string Field3 { get; set; } = string.Empty;
    }
}