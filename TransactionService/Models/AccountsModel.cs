namespace TransactionService.Models;

public class AccountsModel
{
    public static List<Account> Accounts = new List<Account>()
    {
        new Account()
        {
            UserID = 1,
            FullName = "Nils Meijer",
            Email = "nils@gmail.com",
            Password = "password123",
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
            UserID = 2,
            FullName = "Test User",
            Email = "testUser@gmail.com",
            Password = "password567",
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