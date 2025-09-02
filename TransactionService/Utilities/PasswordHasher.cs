using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace TransactionService.Utilities;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int DegreeOfParallelism = 8;
    private const int Iterations = 4;
    private const int MemorySize = 1024 * 1024;
    
    public static string HashPassword(string password)
    {
        //generate the salt and hash
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash = HashPassword(password, salt);

        //Combine the hash and salt into one array, where the salt is placed at index 0 and the hash is placed after the salt.
        var combinedBytes = new byte[salt.Length + hash.Length];
        Array.Copy(salt, 0, combinedBytes, 0, salt.Length);
        Array.Copy(hash, 0, combinedBytes, salt.Length, hash.Length);

        return Convert.ToBase64String(combinedBytes);
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        var argon2id = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            Iterations = Iterations,
            MemorySize = MemorySize
        };

        return argon2id.GetBytes(HashSize);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] combinedBytes = Convert.FromBase64String(hashedPassword);
        
        byte[] salt = new byte[SaltSize];
        byte[] hash = new byte[HashSize];
        
        //extract the hash and salt from the combined array
        Array.Copy(combinedBytes, 0, salt, 0, SaltSize);
        Array.Copy(combinedBytes, SaltSize, hash, 0, HashSize);

        byte[] newHash = HashPassword(password, salt);
        
        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }
}