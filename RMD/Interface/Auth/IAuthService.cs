using RMD.Models.Login;
using RMD.Models.Responses;
using RMD.Models.Usuarios;
using RMD.Service.Auth;
using System;
using System.Threading.Tasks;

namespace RMD.Interface.Auth
{
    public interface IAuthService
    {
        Task<ResponseFromService<string>> LoginAsync(UserCredentials credentials);
        Task<ResponseFromService<bool>> LogoutAsync(string token);
        Task<ResponseFromService<string>> RenewTokenAsync();
        Task<ResponseFromService<string>> GeneratePasswordResetTokenAsync(Guid userId);
        Task<ResponseFromService<ResetTokenValidationResult>> ValidateResetTokenAsync(string token);
        Task<ResponseFromService<bool>> UpdatePasswordAsync(Guid userId, string newPassword);
        Task<ResponseFromService<bool>> IsTokenActiveAsync(string token);
        Task<ResponseFromService<bool>> RevokeTokenAsync(string token, DateTime expirationDate);
        Task<ResponseFromService<string>> ChangeSucursalAsync(Guid newSucursalId);
        Task<ResponseFromService<string>> ChangeGempAsync(Guid newGempId);
        //Task<ResponseFromService<bool>> Send2FACodeAsync();
        //Task<ResponseFromService<bool>> Validate2FACodeAsync(string code);
    }
}
