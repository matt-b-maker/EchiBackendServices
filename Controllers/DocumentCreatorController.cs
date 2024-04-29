using System.Net;
using EchiBackendServices.Models;
using EchiBackendServices.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

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

        [HttpPost]
        public async Task<IActionResult> CreateInspectionReport([FromBody] InspectionReportModel inspectionReportModel)
        {
            try
            {
                if (inspectionReportModel is
                    {
                        InspectionReportLines: not null, Client: not null
                    })
                {
                    var reportUrl = await documentService.CreateInspectionReport(inspectionReportModel.Client,
                        inspectionReportModel.InspectionReportLines, inspectionReportModel.Images);
                    return Ok(reportUrl);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}
