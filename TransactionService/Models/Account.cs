using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TransactionService.Utilities;

namespace TransactionService.Models;

public class Account
{
    //Not created by the user but by the system using UUID like we discussed but not necessary for now.
    //Can just work with incrementing integers for now.
    public int ID { get; set; }
    public required string FullName { get; set; }
    
    [EmailValidation]
    [Required]
    public required string Email { get; set; }
    
    public required string Password { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }

    public ICollection<AccountData> Data { get; set; } = new List<AccountData>();
}

public enum AccountType
{
    Student,
    Shared,
    Youth,
    YoungAdult,
}