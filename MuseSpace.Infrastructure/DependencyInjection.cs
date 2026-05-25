using Microsoft.Extensions.DependencyInjection;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Repositories;

namespace MuseSpace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}