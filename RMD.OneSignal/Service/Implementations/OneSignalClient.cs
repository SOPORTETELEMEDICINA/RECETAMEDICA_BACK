using OneSignalSDK.DotNet;
using RMD.OneSignal.Models;
using RMD.OneSignal.Service.Interfaces;

namespace RMD.OneSignal.Service.Implementations;

public sealed class OneSignalClient : IOneSignalClient
{
    private readonly OneSignalOptions _opt;

    public OneSignalClient(OneSignalOptions options)
    {
        _opt = options;
        OneSignal.Default.Initialize(_opt.AppId);
    }

    public Task LoginAsync(string externalUserId)
    {
        OneSignal.Default.Login(externalUserId);
        return Task.CompletedTask;
    }

    public Task LogoutAsync()
    {
        OneSignal.Default.Logout();
        return Task.CompletedTask;
    }

    public async Task<string?> GetDeviceIdAsync()
    {
        var state = await OneSignal.Default.User.GetOnesignalIdAsync();
        return state;
    }
}