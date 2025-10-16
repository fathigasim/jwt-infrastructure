using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace JwtInfrastructure.Helpers
{
    public static class Argon2Helper
    {
        private const int SaltSize = 16;   // 128-bit salt
        private const int HashSize = 32;   // 256-bit hash
        private const int MemorySize = 65536; // 64 MB
        private const int Iterations = 4;  // time cost
        private const int Parallelism = 8; // CPU threads

        public static string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = Parallelism,
                Iterations = Iterations,
                MemorySize = MemorySize
            };

            var hash = argon2.GetBytes(HashSize);

            // Combine salt + hash
            var result = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

            return Convert.ToBase64String(result);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var decoded = Convert.FromBase64String(storedHash);
            var salt = new byte[SaltSize];
            var hash = new byte[HashSize];

            Buffer.BlockCopy(decoded, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(decoded, SaltSize, hash, 0, HashSize);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = Parallelism,
                Iterations = Iterations,
                MemorySize = MemorySize
            };

            var computed = argon2.GetBytes(HashSize);
            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}
