namespace TransactionService.Models;

public class Password
{
    public int Id { get; set; }
    public int AccountID { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
    public DateTime CreatedAt { get; set; }
}