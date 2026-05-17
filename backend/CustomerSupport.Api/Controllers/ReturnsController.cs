using CustomerSupport.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupport.Api.Controllers;

[ApiController]
[Route("api/returns")]
public class ReturnsController : ControllerBase
{
    private readonly ICustomerSupportRepository _repository;

    public ReturnsController(ICustomerSupportRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("pending-refunds")]
    public async Task<IActionResult> GetPendingRefunds()
    {
        return Ok(await _repository.GetPendingRefundsAsync());
    }
}