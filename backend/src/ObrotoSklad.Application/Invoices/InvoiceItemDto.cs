using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;

namespace ObrotoSklad.Application.Invoices;

public record class InvoiceItemDto(int Id, int InvoiceId, Invoice Invoice, int ProductId, Product Product, int Quantity, decimal UnitPrice, decimal VatRate, decimal NetAmount, decimal GrossAmount);

