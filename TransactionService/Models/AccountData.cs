namespace TransactionService.Models;

public class AccountData
{
    public int ID { get; set; }
    public Account Account { get; set; }
    public float Balance { get; set; }
    public AccountType Type { get; set; }
}