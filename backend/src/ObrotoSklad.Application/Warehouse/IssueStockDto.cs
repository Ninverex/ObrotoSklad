namespace ObrotoSklad.Application.Warehouse;

public record class IssueStockDto(int ProductId, int Quantity, int OrderId);

