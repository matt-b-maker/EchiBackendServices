using EchiBackendServices.Models;
using EchiBackendServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EchiBackendServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DocumentCreatorController(DocumentService documentService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateInspectionAgreement([FromBody] ClientModel clientModel)
        {
            try
            {
                if (!clientModel.IncludeRadonAddendum)
                {
                    var documentUrl = await documentService.CreateInspectionAgreement(clientModel);
                    return Ok(new[] { documentUrl });
                }
                var inspectionAgreementUrl = await documentService.CreateInspectionAgreement(clientModel);
                var radonAddendumUrl = await documentService.CreateRadonAddendum(clientModel);
                return Ok(new[] { inspectionAgreementUrl, radonAddendumUrl });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
