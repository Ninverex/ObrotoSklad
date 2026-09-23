namespace ObrotoSklad.Application.Warehouse;

public record class ReleaseReservationDto(int ProductId, int Quantity, int OrderId);