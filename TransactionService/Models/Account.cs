using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransactionService.Utilities;

namespace TransactionService.Models;

public class Account
{
    //Not created by the user but by the system using UUID like we discussed but not necessary for now.
    //Can just work with incrementing integers for now.
    [JsonPropertyName("id")] public int ID { get; set; }
    
    [JsonPropertyName("fullName")] public required string FullName { get; set; }
    [JsonPropertyName("email")] public required string Email { get; set; }
    [JsonPropertyName("password")] public required string Password { get; set; }
    [Phone] [JsonPropertyName("phoneNumber")] public string PhoneNumber { get; set; }

    public ICollection<AccountData> Data { get; set; } = new List<AccountData>();
}

public enum AccountType
{
    Student,
    Shared,
    Youth,
    YoungAdult,
}