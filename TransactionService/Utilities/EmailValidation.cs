namespace TransactionService.Utilities;

//Like discussed earlier we can also not use this utility class but instead create an EmailAddress class
//and let it handle its own validation, and then the email address data is stored in this class as well.
public static class EmailValidation
{
    private static List<string> _domainWhitelist = new List<string>
    {
        "gmail.com",
        "yahoo.com",
        "outlook.com",
        "hotmail.com",
        "live.com"
    };
    
    //Not sure which approach is better, throwing exceptions, returning true/false or returning a result object.
    //Especially with unit tests in mind. Because I have found that exceptions are harder to test?
    //Whereas only returning true or false results in a loss of information about why the validation failed.
    //EmailValidationResult okay approach?
    public static ContactInformationValidationResult IsValidEmail(string email)
    {
        ContactInformationValidationResult result = new ContactInformationValidationResult()
        {
            IsValid = false
        };

        if (string.IsNullOrEmpty(email))
        {
            result.Message = "Email address cannot be null or empty.";
            return result;
        }

        if (!email.Contains("@") || !email.Contains("."))
        {
            result.Message = "Email address must contain '@' and '.' characters.";
            return result;
        }

        //Extract the domain part of the email using the index of @ + 1 as a starting point
        //and then check if its in the whitelist
        var atIndex = email.IndexOf('@');
        var domain = email.Substring(atIndex + 1);
        if (!_domainWhitelist.Contains(domain))
        {
            result.Message = "Domain is not whitelisted.";
            return result;
        }

        result.IsValid = true;
        result.Message = "Email address is valid.";
        return result;
    }
}