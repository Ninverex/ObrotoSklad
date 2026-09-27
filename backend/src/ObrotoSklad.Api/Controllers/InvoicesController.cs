using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObrotoSklad.Application.Invoices;

namespace ObrotoSklad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        public readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<InvoiceDto>>> GetAllInvoices()
        {
            var invoice = await _invoiceService.GetAllAsync();
            return Ok(invoice);
        }

        [HttpGet("{invoiceId}")]
        [Authorize]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int invoiceId)
        {
            var invoice = await _invoiceService.GetByIdAsync(invoiceId);

            if (invoice is null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }
    }
}
