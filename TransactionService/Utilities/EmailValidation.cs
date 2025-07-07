namespace TransactionService.Utilities;

public static class Email
{
    private static List<string> _domainWhitelist = new List<string>
    {
        "gmail.com",
        "yahoo.com",
        "outlook.com",
        "hotmail.com",
        "live.com"
    };

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");

        if (!email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Invalid email address format.");

        //Extract the domain part of the email using the index of @ + 1 as a starting point
        //and then check if its in the whitelist
        var atIndex = email.IndexOf('@');
        var domain = email.Substring(atIndex + 1);
        if (!_domainWhitelist.Contains(domain))
            throw new ArgumentException("Email domain is not whitelisted.");

        return true;
    }
}