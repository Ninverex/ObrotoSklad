namespace ObrotoSklad.Application.Customers;

public record class CustomerDto(int Id, string Name, string NIP, string Address, string Mail, string PhoneNumber);
