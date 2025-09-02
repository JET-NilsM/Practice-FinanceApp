using TransactionService.Utilities;

namespace TransactionService.DTO;

public class AccountDTO
{
    public int ID { get; set; }
    public string FullName { get; set; }
    [EmailValidation] public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}