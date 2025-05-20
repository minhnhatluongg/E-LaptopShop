using E_LaptopShop.Domain.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class PasswordHasher : Domain.Repositories.IPasswordHasher
    {
        // Số lần lặp thuật toán - càng cao càng an toàn nhưng tốn thời gian tính toán
        private const int IterationCount = 10000;
        // Độ dài salt (16 bytes = 128 bits)
        private const int SaltSize = 16;
        // Độ dài hash (32 bytes = 256 bits)
        private const int HashSize = 32;

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a subkey using PBKDF2 with HMACSHA256
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize);

            // Combine salt and hash into a single string (format: algorithm:iterations:salt:hash)
            byte[] combinedBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, combinedBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, combinedBytes, SaltSize, HashSize);

            // Return base64-encoded string
            return $"PBKDF2:HMACSHA256:{IterationCount}:{Convert.ToBase64String(combinedBytes)}";
        }

        public bool VerifyHashedPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentNullException(nameof(passwordHash));

            try
            {
                // Parse the hash string
                string[] parts = passwordHash.Split(':');

                // Validate format
                if (parts.Length != 4 ||
                    parts[0] != "PBKDF2" ||
                    parts[1] != "HMACSHA256")
                    return false;

                // Parse iteration count
                if (!int.TryParse(parts[2], out int iterations))
                    return false;

                // Decode combined salt and hash
                byte[] combinedBytes = Convert.FromBase64String(parts[3]);
                if (combinedBytes.Length != SaltSize + HashSize)
                    return false;

                // Extract salt and hash
                byte[] salt = new byte[SaltSize];
                byte[] storedHash = new byte[HashSize];
                Buffer.BlockCopy(combinedBytes, 0, salt, 0, SaltSize);
                Buffer.BlockCopy(combinedBytes, SaltSize, storedHash, 0, HashSize);

                // Compute hash with the same salt
                byte[] computedHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: iterations,
                    numBytesRequested: HashSize);

                // Compare computed hash with stored hash
                return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
            }
            catch
            {
                // Any exception during verification process should result in failure
                return false;
            }
        }
    }
}
