using CustomerSupport.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupport.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerSupportRepository _repository;

    public CustomersController(ICustomerSupportRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetCustomerById(int customerId)
    {
        var result = await _repository.GetCustomerByIdAsync(customerId);
        return result == null ? NotFound(new { message = "Customer not found." }) : Ok(result);
    }

    [HttpGet("{customerId:int}/addresses")]
    public async Task<IActionResult> GetCustomerAddresses(int customerId)
    {
        return Ok(await _repository.GetCustomerAddressesAsync(customerId));
    }

    [HttpGet("{customerId:int}/overview")]
    public async Task<IActionResult> GetCustomerOverview(int customerId)
    {
        var result = await _repository.GetCustomerOverviewAsync(customerId);
        return result == null ? NotFound(new { message = "Customer not found." }) : Ok(result);
    }

    [HttpGet("{customerId:int}/orders")]
    public async Task<IActionResult> GetCustomerOrders(int customerId)
    {
        return Ok(await _repository.GetCustomerOrderHistoryAsync(customerId));
    }
}