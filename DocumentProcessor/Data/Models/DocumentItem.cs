namespace DocumentProcessor.Data.Models
{
    public class DocumentItem
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal NetPrice { get; set; }
        public decimal NetValue { get; set; }
        public decimal VatValue { get; set; }
        public decimal QuantityBefore { get; set; }
        public decimal AvgBefore { get; set; }
        public decimal QuantityAfter { get; set; }
        public decimal AvgAfter { get; set; }
        public string Group { get; set; } = string.Empty;
    }
}