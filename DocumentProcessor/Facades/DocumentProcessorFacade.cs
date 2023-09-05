namespace DocumentProcessor.Facades
{
    using DocumentProcessor.Data.Commands;
    using DocumentProcessor.Services;

    public class DocumentProcessorFacade : IDocumentProcessorFacade
    {
        private readonly IDocumentProcessorService _documentProcessorService;

        public DocumentProcessorFacade(IDocumentProcessorService documentProcessorService)
        {
            _documentProcessorService = documentProcessorService;
        }

        public DocumentProcessResponse ProcessDocument(DocumentProcessCommand command)
        {
            return _documentProcessorService.ProcessDocument(command);
        }
    }
}