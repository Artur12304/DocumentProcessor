namespace DocumentProcessor.Controllers
{
    using DocumentProcessor.Data.Commands;
    using DocumentProcessor.Facades;
    using Microsoft.AspNetCore.Mvc;
    using System.Text;

    [ApiController]
    [Route("api/test")]
    public class DocumentProcessorController : ControllerBase
    {
        private const string ValidUsername = "vs";
        private const string ValidPassword = "rekrutacja";

        private readonly IDocumentProcessorFacade _documentProcessorFacade;

        public DocumentProcessorController(IDocumentProcessorFacade documentProcessorFacade)
        {
            _documentProcessorFacade = documentProcessorFacade;
        }

        [HttpPost("{x}")]
        public async Task<IActionResult> ProcessFile(IFormFile file, int x)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (!IsAuthorized(authHeader))
                {
                    return Unauthorized();
                }

                if (file == null)
                {
                    return BadRequest("No file uploaded.");
                }

                if (!IsFileExtensionValid(file.FileName, validExtension: ".PUR"))
                {
                    return BadRequest("Invalid file format. Only .PUR files are allowed.");
                }

                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    if (string.IsNullOrWhiteSpace(requestBody))
                    {
                        return BadRequest("No input data.");
                    }

                    var command = new DocumentProcessCommand
                    {
                        RequestBody = requestBody,
                        NumberOfItems = x
                    };

                    var response = await _documentProcessorFacade.ProcessDocument(command);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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

        private static bool IsAuthorized(string authHeader)
        {
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];

                return username == ValidUsername && password == ValidPassword;
            }

            return false;
        }
    }
}