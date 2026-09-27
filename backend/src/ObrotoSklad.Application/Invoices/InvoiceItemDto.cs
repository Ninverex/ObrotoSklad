using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;

namespace ObrotoSklad.Application.Invoices;

public record InvoiceItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal VatRate, decimal NetAmount, decimal GrossAmount);
