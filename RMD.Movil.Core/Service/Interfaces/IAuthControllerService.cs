using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Login;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IAuthControllerService
    {
        Task<ResponseFromService<LoginResult>> LoginAsync(UserCredentials credentials);
        Task<ResponseFromService<bool>> LogoutAsync();
        Task<ResponseFromService<TokenRenewResult>> RenewTokenAsync();
        //Task<ResponseFromService<TokenRenewResult>> RenewTokenAsync(string tokekEncrypt);
        Task<ResponseFromService<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ResponseFromService<string>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}