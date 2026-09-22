namespace ObrotoSklad.Application.Customers;

public record class CreateCustomerDto(string Name, string NIP, string Adress, string Mail, string PhoneNumber);

