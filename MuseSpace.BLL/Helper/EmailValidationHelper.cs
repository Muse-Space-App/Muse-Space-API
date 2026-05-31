using System.Net.Mail;

namespace MuseSpace.BLL.Helper;

public static class EmailValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);
            return address.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}