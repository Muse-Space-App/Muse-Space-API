namespace MuseSpace.Infrastructure.ExternalServices;

public interface IEmailTemplateService
{
    string BuildWelcomeBody(string displayName);
}

public sealed class EmailTemplateService : IEmailTemplateService
{
    public string BuildWelcomeBody(string displayName)
    {
        return $"Hello {displayName}, welcome to MuseSpace.";
    }
}
