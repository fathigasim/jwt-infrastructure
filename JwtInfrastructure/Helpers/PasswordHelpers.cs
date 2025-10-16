
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
namespace JwtInfrastructure.Helpers
{
    // Install-Package BCrypt.Net-Next

    

    public static class PasswordHelpers
    {
        // Hash password
        const int keySize = 64;
        const int iterations = 350000;
        static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

     public static  string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }
    }

}
