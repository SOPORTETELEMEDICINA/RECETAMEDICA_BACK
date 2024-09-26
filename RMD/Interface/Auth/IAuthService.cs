using RMD.Models.Login;

namespace RMD.Interface.Auth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(UserCredentials credentials);
        Task LogoutAsync(string token);
        Task<string> RenewTokenAsync();
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task<(bool IsValid, string ErrorMessage, Guid UserId)> ValidateResetTokenAsync(string token);
        Task<bool> UpdatePasswordAsync(Guid userId, string newPassword);
        Task<bool> IsTokenActiveAsync(string token);
        Task RevokeTokenAsync(string token, DateTime expirationDate);
    }
}
