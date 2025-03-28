using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Helpers
{
    public class LoginHelper
    {
        // Method to generate an MD5 hash from a password
        public static string HashGen(string password)
        {
            // Create an instance of the MD5 cryptographic provider
            MD5 md5 = new MD5CryptoServiceProvider();

            // Append a salt ("internship") to the password and convert it to bytes
            var originalBytes = Encoding.Default.GetBytes(password + "internship");

            // Compute the MD5 hash of the byte array
            var encodedBytes = md5.ComputeHash(originalBytes);

            // Convert the hash bytes to a hexadecimal string and return it
            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }
    }
}
