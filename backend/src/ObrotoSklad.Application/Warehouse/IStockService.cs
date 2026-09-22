namespace ObrotoSklad.Application.Warehouse;

public interface IStockService
{
    Task<List<StockItemDto>> GetAllStockItemDtosAsync();
    Task<StockItemDto?> GetStockItemDtoByProductIdAsync(int productId);
    Task<List<StockMovementDto>> GetMovementHistoryAsync(int productId);

    Task<StockItemDto> ReceiveStockAsync(ReceiveStockDto dto, string userId);
    Task ReserveStockAsync(ReserveStockDto dto, string userId);
    Task ReleaseReservationAsync(int productId, int quantity, int orderId, string userId);
    Task IssueStockAsync(IssueStockDto dto, string userId);
}
