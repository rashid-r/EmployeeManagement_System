using System;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagementSystem.Services
{
    public static class BCryptSimulatedHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return $"$bcryptsim${Iterations}${hashedPassword}";
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Extract parts from the stored hash
                var parts = hashedPassword.Split('$');
                if (parts.Length != 4 || parts[1] != "bcryptsim")
                    return false;

                int iterations = int.Parse(parts[2]);
                string hashString = parts[3];

                // Convert base64 string to bytes
                byte[] hashBytes = Convert.FromBase64String(hashString);

                // Extract salt
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Compute hash of the provided password
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
                byte[] testHash = pbkdf2.GetBytes(HashSize);

                // Compare hashes
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != testHash[i])
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}