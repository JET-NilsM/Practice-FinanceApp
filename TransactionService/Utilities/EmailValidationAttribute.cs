using System.ComponentModel.DataAnnotations;

namespace TransactionService.Utilities;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class EmailValidationAttribute : ValidationAttribute
{
    private static List<string> _domainWhitelist = new List<string>
    {
        "gmail.com",
        "yahoo.com",
        "outlook.com",
        "hotmail.com",
        "live.com"
    };

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Value cannot be null");
        }

        if (!(value is string valueAsString))
        {
            return new ValidationResult("Value is not a string");
        }
        
        if (valueAsString.AsSpan().ContainsAny('\r', '\n'))
        {
            return new ValidationResult("Value contains linebreaks");
        }
        
        int atIndex = valueAsString.IndexOf('@');
        
        if (atIndex < 0 || 
            atIndex == valueAsString.Length - 1 ||
            atIndex != valueAsString.LastIndexOf('@'))
        {
            return new ValidationResult("Value format is invalid");
        }

        if (!_domainWhitelist.Contains(valueAsString.Substring(atIndex + 1)))
        {
            return new ValidationResult("Provided domain is not whitelisted.");
        }        

        return ValidationResult.Success;
    }
}