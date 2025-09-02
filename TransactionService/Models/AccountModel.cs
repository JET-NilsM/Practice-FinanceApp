using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransactionService.Utilities;

namespace TransactionService.Models;

public class AccountModel
{
    //Not created by the user but by the system using UUID like we discussed but not necessary for now.
    //Can just work with incrementing integers for now.
    [JsonPropertyName("id")] public int ID { get; set; }
    [JsonPropertyName("fullName")] [Required] public string FullName { get; set; }
    [JsonPropertyName("email")] [Required] [EmailValidation] public string Email { get; set; }
    [JsonPropertyName("password")] public string Password { get; set; }
    [JsonPropertyName("phoneNumber")] public string PhoneNumber { get; set; }

    public ICollection<AccountData> Data { get; set; } = new List<AccountData>();
}

public enum AccountType
{
    Student,
    Shared,
    Youth,
    YoungAdult,
}