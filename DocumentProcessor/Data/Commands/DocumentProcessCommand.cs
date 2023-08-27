namespace DocumentProcessor.Data.Commands
{
    public class DocumentProcessCommand
    {
        public string RequestBody { get; set; } = string.Empty;
        public int NumberOfItems { get; set; }
    }
}
