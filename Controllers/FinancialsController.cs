using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiProject.Services;

namespace MyApiProject.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FinancialsController : ControllerBase
{
    private readonly IFinancialService _service;

    public FinancialsController(IFinancialService service) => _service = service;

    [HttpGet("report")]
    public async Task<ActionResult<object>> GetReport() => Ok(await _service.GetFinancialReportAsync());
}
