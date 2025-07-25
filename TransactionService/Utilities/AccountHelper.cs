using System.Text.Json;
using TransactionService.Models;

namespace TransactionService.Utilities;

public static class AccountHelper
{
    public static Object? ConvertToProperty(KeyValuePair<string, object> dataEntry, Account account)
    {
        // Check if the property exists in the model
        var property = typeof(Account).GetProperty(dataEntry.Key);
        if (property == null || !property.CanWrite)
        {
            return null;
        }

        
        object? convertedValue;
        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (dataEntry.Value is JsonElement jsonElement)
        {
            object? rawValue = jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => targetType == typeof(int),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            };
            convertedValue = Convert.ChangeType(rawValue, targetType);
        }
        else
        {
            convertedValue = Convert.ChangeType(dataEntry.Value, targetType);
        }

        Console.WriteLine($"Value: {convertedValue}");
        property.SetValue(account, convertedValue);

        return convertedValue;
    }
}