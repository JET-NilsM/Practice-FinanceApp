using System.ComponentModel.DataAnnotations;

namespace TransactionService.Utilities;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class EmailValidationAttribute : DataTypeAttribute
{
    private static List<string> _domainWhitelist = new List<string>
    {
        "gmail.com",
        "yahoo.com",
        "outlook.com",
        "hotmail.com",
        "live.com"
    };
    
    //Convert to primary constructor? 
    public EmailValidationAttribute() : base(DataType.EmailAddress)
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }
        
        if (!(value is string valueAsString))
        {
            return false;
        }
        
        if (valueAsString.AsSpan().ContainsAny('\r', '\n'))
        {
            return false;
        }
        
        int index = valueAsString.IndexOf('@');
        
        if (index < 0 || 
            index == valueAsString.Length - 1 ||
            index != valueAsString.LastIndexOf('@'))
        {
            return false;
        }
        
        if (!_domainWhitelist.Any(ending => 
                valueAsString.AsSpan(index + 1)
                .EndsWith(ending.AsSpan(), StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        return true;
    }
}