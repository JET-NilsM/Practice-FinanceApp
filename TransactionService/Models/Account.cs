namespace TransactionService.Models;

public class Account
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public EmailAddress Email { get; set; }
    public string Password { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public float AccountBalance { get; set; }
    public AccountType AccountType { get; set; }
}

public enum AccountType
{
    Student,
    Shared,
    Youth,
    YoungAdult,
}