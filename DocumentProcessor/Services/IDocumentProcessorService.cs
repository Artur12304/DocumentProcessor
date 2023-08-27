namespace DocumentProcessor.Services
{
    using DocumentProcessor.Data.Commands;

    public interface IDocumentProcessorService
    {
        Task<DocumentProcessResponse> ProcessDocument(DocumentProcessCommand command);
    }
}