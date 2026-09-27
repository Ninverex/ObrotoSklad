using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Enums;

namespace ObrotoSklad.Application.Invoices;

public record class InvoiceDto(int Id, string InvoiceNumber, int OrderId, string OrderNumber,
    string CustomerName, DateOnly IssueDate, DateOnly DueDate, InvoiceStatus Status, decimal TotalNet, decimal TotalVat, decimal TotalGross, List<InvoiceItemDto> Items);
