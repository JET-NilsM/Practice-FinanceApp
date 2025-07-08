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
        //
        // //The already written IsValid method uses AsSpan() rather than Substring.
        // //From online sources (https://medium.com/c-sharp-programming/c-tip-prefer-asspan-over-substring-for-performance-6ee1ee1edee9)
        // //I gather that AsSpan() is much more efficient with large scale
        // //operations, but I don't think we are validating email addresses 100.000 times per second?
        // //Wondering why that was being used in the EmailAddressAttribute class.
        // //Copilot output:
        // //The main advantage of using `AsSpan()` in this context is not just performance, but also code clarity and safety:
        // // 
        // // - **No unnecessary string allocations:** Even if performance is not critical, avoiding temporary string allocations (e.g., with `Substring`) is a good practice, especially in libraries or reusable components.
        // // - **Direct, safe access:** `AsSpan()` allows you to work directly with slices of the original string, reducing the risk of bugs from incorrect substring calculations.
        // // - **Consistency:** Using `Span`-based APIs can make the codebase more modern and consistent, especially if other parts use spans for parsing or validation.
        // // 
        // // For most business applications, the performance gain is minor, but using `AsSpan()` is a forward-looking, safe, and efficient pattern.
        if (valueAsString.AsSpan().ContainsAny('\r', '\n'))
        {
            return false;
        }
        //
        // // only return true if there is only 1 '@' character
        // // and it is neither the first nor the last character
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