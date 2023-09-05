namespace DocumentProcessor.Services
{
    using DocumentProcessor.Data.Commands;

    public interface IDocumentProcessorService
    {
        DocumentProcessResponse ProcessDocument(DocumentProcessCommand command);
    }
}