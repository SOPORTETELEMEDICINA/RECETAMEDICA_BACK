using Microsoft.Extensions.DependencyInjection;
using RMD.OneSignal.Models;
using RMD.OneSignal.Service.Interfaces;
using RMD.OneSignal.Service.Implementations;

namespace RMD.OneSignal.Extensions;

public static class MauiOneSignalExtensions
{
    public static IServiceCollection AddRmdOneSignal(this IServiceCollection services, Action<OneSignalOptions> configure)
    {
        var options = new OneSignalOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddHttpClient<INotificationsService, NotificationsService>();
        services.AddSingleton<IOneSignalClient, OneSignalClient>();

        return services;
    }
}