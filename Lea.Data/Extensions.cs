using Lea.Data;
using Lea.Data.Repositories;
using Lea.Data.Services;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddOracleServices(this IServiceCollection services, IConfiguration configuration, string connectionStringName)
    {
        ArgumentNullException.ThrowIfNull(services);

        var connectionString = configuration.GetConnectionString(connectionStringName);
        services.AddTransient<IDbConnectionFactory, OracleDbConnectionFactory>(serviceProvider =>
        {
            return new OracleDbConnectionFactory(() => new OracleConnection(connectionString));
        });

        // see https://github.com/khellang/Scrutor
        services.Scan(scan => scan
            // // We start out with all types in the assembly of OracleDbConnectionFactory
            .FromAssemblyOf<OracleDbConnectionFactory>()
                // AddClasses starts out with all public, non-abstract types in this assembly.
                // These types are then filtered by the delegate passed to the method.
                // In this case, we filter out only the classes that are assignable to IRepository.
                .AddClasses(classes => classes.AssignableTo<IRepository>())
                // We then specify what type we want to register these classes as.
                // In this case, we want to register the types as all of its implemented interfaces.
                // So if a type implements 3 interfaces; A, B, C, we'd end up with three separate registrations.
                .AsImplementedInterfaces()
                // And lastly, we specify the lifetime of these registrations.
                .WithTransientLifetime());

        // example of explicit registration
        services.AddTransient<IEmployeeService, EmployeeService>();

        return services;
    }
}
