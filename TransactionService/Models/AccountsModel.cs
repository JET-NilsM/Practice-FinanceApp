namespace TransactionService.Models;

public class AccountsModel
{
    public static List<Account> Accounts = new List<Account>()
    {
        new Account()
        {
            ID = 1,
            FullName = "Nils Meijer",
            Email = "nils@gmail.com",
            PhoneNumber =  "+31 6 12345678",
            Data = new List<AccountData>()
            {
                new AccountData()
                {
                    Balance = 100.0f,
                    Type = AccountType.Student
                }
            }
        },
        new Account()
        {
            ID = 2,
            FullName = "Test User",
            Email = "testUser@gmail.com",
            PhoneNumber =  "+31 6 12345678",
            Data = new List<AccountData>()
            {
                new AccountData()
                {
                    Balance = 100.0f,
                    Type = AccountType.Student
                }
            }
        },
    };
}