using System.Text;

namespace WebApplication1.Helpers
{
    public class PasswordGenerator
    {
        // Create a static instance of Random for generating random numbers
        private static readonly Random random = new();

        // Method to generate a random password of specified length
        public static string GeneratePassword(int length = 12)
        {
            // Define the characters that can be included in the password
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";

            // Initialize a StringBuilder to build the password
            StringBuilder result = new(length);

            // Generate a random password by selecting characters from validChars
            for (int i = 0; i < length; i++)
            {
                // Append a random character to the result
                result.Append(validChars[random.Next(validChars.Length)]);
            }

            // Return the generated password as a string
            return result.ToString();
        }
    }
}
