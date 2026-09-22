namespace ObrotoSklad.Application.Customers;

public record class CreateCustomerDto(string Name, string NIP, string Address, string Mail, string PhoneNumber);

