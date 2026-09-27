namespace ObrotoSklad.Application.Invoices;

public interface IInvoiceService
{
    Task<InvoiceDto> GenerateFromOrderAsync(int orderId);
    Task<InvoiceDto?> GetByIdAsync(int id);
    Task<List<InvoiceDto>> GetAllAsync();
}
