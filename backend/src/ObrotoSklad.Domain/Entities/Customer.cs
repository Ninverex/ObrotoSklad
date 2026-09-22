using System;
using System.Net.Mail;

namespace ObrotoSklad.Domain;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string NIP { get; set; }="";
    public string Address { get; set; }="";
    public required string Mail { get; set; }
    public required string PhoneNumber { get; set; }
}
