namespace MuseSpace.Core.Interfaces.Helper;

public interface IPasswordHashHelper
{
    /// <summary>
    /// Hashes a plaintext password using a secure hashing algorithm (e.g., bcrypt, Argon2). The resulting hash should include a unique salt and be computationally expensive to deter brute-force attacks.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    string HashPassword(string password);
    
    /// <summary>
    /// Verifies a plaintext password against a hashed password.
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    bool VerifyPassword(string password, string hash);
}
