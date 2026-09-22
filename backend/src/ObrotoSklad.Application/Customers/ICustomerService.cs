namespace ObrotoSklad.Application.Customers;

public interface ICustomerService
{   
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto dto);
    Task<bool> DeleteAsync(int id);

}
