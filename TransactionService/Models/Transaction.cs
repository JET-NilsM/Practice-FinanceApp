namespace TransactionService.Models;

public class Transaction
{
    public int ID { get; set; }
    public int SenderID { get; set; }
    public int ReceiverID { get; set; }
    public float Amount { get; set; }
    public string Description { get; set; }
}