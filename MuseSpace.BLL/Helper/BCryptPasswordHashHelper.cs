using BCrypt.Net;
using MuseSpace.Core.Interfaces.Helper;

namespace MuseSpace.BLL.Helper;

public sealed class BCryptPasswordHashHelper : IPasswordHashHelper
{
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        try
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Password hashing failed", ex);
        }
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Password verification error: {ex.Message}");
            return false;
        }
    }
}
