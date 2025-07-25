using System.Text.Json.Serialization;

namespace TransactionService.Models;

[Serializable]
public class AccountData
{
    [JsonPropertyName("id")] public int ID { get; set; }
    [JsonPropertyName("accountID")] public int AccountID { get; set; }
    [JsonPropertyName("balance")] public float Balance { get; set; }
    [JsonPropertyName("type")] public AccountType Type { get; set; }
}