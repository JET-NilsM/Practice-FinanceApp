```mermaid
classDiagram
direction TB
    class User {
    }

    class AccountController {
	    +CreateAccount()
	    +GetAccount(int id)
	    +GetAccounts()
	    +UpdateAccount(int id, Account newData)
	    +UpdateAccount(int id, AccountData newData)
	    +DeleteAccount(int id)
    }

    class Account {
	    +int ID
	    +String FullName
	    +String Email
	    +string Password
	    +string PhoneNumber
	    +List~AccountData~ Data
    }

    class AccountData {
	    +int ID
	    +int AccountID
	    +float Balance
	    +AccountType Type
	    +Account
    }

    class DbContext {
    }

    class FinanceContext {
	    +DbSet~Account~ Accounts
    }

    class AccountType {
	    STUDENT
	    SHARED
	    YOUTH
	    YOUNG_ADULT
    }

    class AccountRepository {
    }

    class IAccountRepository {
	    +GetAccounts() List
	    +GetAccount(int id) Account
	    +AddAccount(Account newAccount)
	    +UpdateAccount(int id, Account newData)
	    +DeleteAccount(int id)
	    +Dispose()
    }

    class AccountHelper {
	    +static ConvertToProperty() : object
    }

	<<enumeration>> AccountType
	<<interface>> IAccountRepository

    AccountController --> IAccountService : depends on
    IAccountService --> IAccountRepository : depends on
    
    AccountController --> AccountDTO : uses
    AccountService --> AccountModel : uses
    AccountRepository --> AccountEntity : uses
    
    User --> AccountController : GET
    User --> AccountController : POST
    User --> AccountController : PUT
    User --> AccountController : DELETE


```