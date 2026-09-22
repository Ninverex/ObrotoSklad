using Microsoft.AspNetCore.Mvc;
using ObrotoSklad.Application.Customers;

namespace ObrotoSklad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        public readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto dto)
        {
            var customer = await _customerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCustomerById), new {id = customer.Id}, customer);
        }

    [HttpGet]
    public async Task<ActionResult<CustomerDto>> GetCustomers()
        {
            var customer = await _customerService.GetAllAsync();
            return Ok(customer);
        }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer is null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, UpdateCustomerDto dto)
        {
            var customer = await _customerService.UpdateAsync(id, dto);
            if (customer is null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerService.DeleteAsync(id);
            if (customer is false)
            {
                return NotFound();
            }
            return NoContent();
        }

}
}

