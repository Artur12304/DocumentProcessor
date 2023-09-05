namespace DocumentProcessor.Facades
{
    using DocumentProcessor.Data.Commands;

    public interface IDocumentProcessorFacade
    {
        DocumentProcessResponse ProcessDocument(DocumentProcessCommand command);
    }
}