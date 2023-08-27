namespace DocumentProcessor.Facades
{
    using DocumentProcessor.Data.Commands;

    public interface IDocumentProcessorFacade
    {
        Task<DocumentProcessResponse> ProcessDocument(DocumentProcessCommand command);
    }
}