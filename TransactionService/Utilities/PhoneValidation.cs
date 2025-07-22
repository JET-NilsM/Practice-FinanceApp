namespace TransactionService.Utilities;

public class PhoneValidation
{
    private static List<string> _validCountryCodes = new List<string>
    {
        "+31", // Netherlands
        "+1",  // USA/Canada
        "+44", // UK
        "+49", // Germany
        "+33"  // France
    };
    
    //A bunch of edge cases possible here, but out of scope for now.
    public static ContactInformationValidationResult IsValidPhoneNumber(string phoneNumber)
    {
        ContactInformationValidationResult result = new ContactInformationValidationResult()
        {
            IsValid = false
        };

        if (string.IsNullOrEmpty(phoneNumber))
        {
            result.Message = "Phone number cannot be null or empty.";
            return result;
        }
        
        string extractedCountryCode = phoneNumber.Split(' ')[0]; // Assuming the country code is the first part of the phone number
        if(!_validCountryCodes.Contains(extractedCountryCode))
        {
            result.Message = "Country code is not valid.";
            return result;
        }

        result.IsValid = true;
        result.Message = "Phone number is valid.";
        return result;
    }
}