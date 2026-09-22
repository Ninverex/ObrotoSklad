using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Application.Common;
using ObrotoSklad.Domain;

namespace ObrotoSklad.Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly IAppDbContext _context;
    public CustomerService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            NIP = dto.NIP,
            Address = dto.Address,
            Mail = dto.Mail,
            PhoneNumber = dto.PhoneNumber
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new CustomerDto(customer.Id, customer.Name, customer.NIP, customer.Address, customer.Mail, customer.PhoneNumber);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = _context.Customers.Find(id);

        if (customer is null)
        {
            return false;
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
       var customers = await _context.Customers.ToListAsync();

       return customers
            .Select(customer => new CustomerDto(customer.Id, customer.Name, customer.NIP, customer.Address, customer.Mail, customer.PhoneNumber))
            .ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer is null)
        {
            return null;
        }
        return new CustomerDto(customer.Id, customer.Name, customer.NIP, customer.Address, customer.Mail, customer.PhoneNumber);
    }

    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        
        if (customer is null)
        {
            return null;
        }

        customer.Name = dto.Name;
        customer.Address = dto.Address;
        customer.Mail = dto.Mail;
        customer.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();

        return new CustomerDto(customer.Id, customer.Name, customer.NIP, customer.Address, customer.Mail, customer.PhoneNumber);

    }
}
