using CustomerSupport.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupport.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ICustomerSupportRepository _repository;

    public ProductsController(ICustomerSupportRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return BadRequest(new { message = "Search text is required." });

        return Ok(await _repository.SearchProductsAsync(searchText));
    }
}