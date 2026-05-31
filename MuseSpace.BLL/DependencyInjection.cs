using Microsoft.Extensions.DependencyInjection;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.BLL.Services;

namespace MuseSpace.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        // Add other services here (e.g., IArtworkService)
        return services;
    }
}