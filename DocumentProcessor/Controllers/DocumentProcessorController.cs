namespace DocumentProcessor.Controllers
{
    using DocumentProcessor.Data.Commands;
    using DocumentProcessor.Facades;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("api/test")]
    public class DocumentProcessorController : ControllerBase
    {
        private readonly IDocumentProcessorFacade _documentProcessorFacade;

        public DocumentProcessorController(IDocumentProcessorFacade documentProcessorFacade)
        {
            _documentProcessorFacade = documentProcessorFacade;
        }

        [HttpPost("{x}")]
        public IActionResult ProcessFile([FromForm] IFormFile file, [FromQuery] int x)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                if (!IsFileExtensionValid(file.FileName, validExtension: ".PUR"))
                {
                    return BadRequest("Invalid file format. Only .PUR files are allowed.");
                }

                var requestBody = ReadRequestBody(file);

                if (string.IsNullOrWhiteSpace(requestBody))
                    return BadRequest("No input data.");

                var command = new DocumentProcessCommand
                {
                    RequestBody = requestBody,
                    NumberOfItems = x
                };

                var response = _documentProcessorFacade.ProcessDocument(command);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private static string ReadRequestBody(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private static bool IsFileExtensionValid(string fileName, string validExtension)
        {
            string fileExtension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(fileExtension))
            {
                return false;
            }

            return fileExtension.Equals(validExtension, StringComparison.OrdinalIgnoreCase);
        }
    }
}