using RMD.Movil.Core.Encryption;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Login;
using RMD.Shared.Models.ServiciosInternos;
using RMD.Shared.Models.Usuarios;
using System.Net.Http.Json;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations;

public class AuthControllerService : IAuthControllerService
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthControllerService(HttpClient client)
    {
        _client = client;
    }
    public async Task<ResponseFromService<LoginResult>> LoginAsync(UserCredentials credentials)
    {
        try
        {
            credentials.Plataform = "MOVIL";

#if DEBUG
            Console.WriteLine($"[LOGIN] URL: {_client.BaseAddress}api/Auth/login");
#endif

            var response = await _client.PostAsJsonAsync("api/Auth/login", credentials);
            var json = await response.Content.ReadAsStringAsync();

#if DEBUG
            Console.WriteLine("[LOGIN] Respuesta cruda:");
            Console.WriteLine(json);
#endif

            var encryptedResponse = JsonSerializer.Deserialize<ResponseFromService<LoginEncryptedData>>(json, JsonOptions)!;

            if (encryptedResponse.Toast?.ToLower() is "error" or "warning")
            {
                return new ResponseFromService<LoginResult>
                {
                    Code = encryptedResponse.Code,
                    Message = encryptedResponse.Message,
                    Toast = encryptedResponse.Toast,
                    Descripcion = encryptedResponse.Descripcion,
                    Data = new()
                };
            }

            var tokenPayload = encryptedResponse.Data.Token;
            var userPayload = encryptedResponse.Data.User;

            var tokenPlain = DecryptService.FromTokenPayload(tokenPayload);
            var user = DecryptService.FromUserPayload<UsuarioDetalle>(userPayload);

            return new ResponseFromService<LoginResult>
            {
                Code = encryptedResponse.Code,
                Message = encryptedResponse.Message,
                Toast = encryptedResponse.Toast,
                Descripcion = encryptedResponse.Descripcion,
                Data = new LoginResult
                {
                    TokenEncrypted = $"{tokenPayload.iv}:{tokenPayload.cipherText}",
                    TokenPlain = tokenPlain,
                    User = user
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseFromService<LoginResult>
            {
                Code = -999,
                Message = "Error al consumir login",
                Toast = "error",
                Descripcion = [ex.Message],
                Data = new()
            };
        }
    }

    public async Task<ResponseFromService<bool>> LogoutAsync()
    {
        try
        {
            var response = await _client.PostAsync("api/Auth/logout", null);
            var json = await response.Content.ReadAsStringAsync();

            var raw = JsonSerializer.Deserialize<ResponseFromService<bool>>(json, JsonOptions)!;

            return new ResponseFromService<bool>
            {
                Code = raw.Code,
                Message = raw.Message,
                Toast = raw.Toast,
                Descripcion = raw.Descripcion,
                Data = raw.Data
            };
        }
        catch (Exception ex)
        {
            return new ResponseFromService<bool>
            {
                Code = -999,
                Message = "Error al consumir logout",
                Toast = "error",
                Descripcion = [ex.Message],
                Data = false
            };
        }
    }


    public async Task<ResponseFromService<TokenRenewResult>> RenewTokenAsync()
    {
        try
        {
            var response = await _client.PostAsync("api/Auth/renew", null);
            var json = await response.Content.ReadAsStringAsync();

            var raw = JsonSerializer.Deserialize<ResponseFromService<string>>(json, JsonOptions)!;

            if (string.IsNullOrWhiteSpace(raw.Data))
                return new ResponseFromService<TokenRenewResult>
                {
                    Code = raw.Code,
                    Message = raw.Message,
                    Toast = raw.Toast,
                    Descripcion = raw.Descripcion,
                    Data = new()
                };

            var partes = raw.Data.Split(':');
            if (partes.Length != 2)
                return new ResponseFromService<TokenRenewResult>
                {
                    Code = raw.Code,
                    Message = "Formato de token inválido",
                    Toast = "error",
                    Descripcion = ["El token no tiene el formato esperado (iv:cipherText)"],
                    Data = new()
                };

            var payload = new EncryptedPayload
            {
                iv = partes[0],
                cipherText = partes[1]
            };

            var tokenPlain = DecryptService.FromTokenPayload(payload);

            return new ResponseFromService<TokenRenewResult>
            {
                Code = raw.Code,
                Message = raw.Message,
                Toast = raw.Toast,
                Descripcion = raw.Descripcion,
                Data = new TokenRenewResult
                {
                    TokenEncrypted = raw.Data,
                    TokenPlain = tokenPlain
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseFromService<TokenRenewResult>
            {
                Code = -999,
                Message = "Error al renovar token",
                Toast = "error",
                Descripcion = [ex.Message],
                Data = new()
            };
        }
    }

    public async Task<ResponseFromService<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/Auth/forgot-password", request);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ResponseFromService<string>>(json, JsonOptions)!;
        }
        catch (Exception ex)
        {
            return new ResponseFromService<string>
            {
                Code = -999,
                Message = "Error al solicitar recuperación",
                Toast = "error",
                Descripcion = [ex.Message],
                Data = string.Empty
            };
        }
    }

    public async Task<ResponseFromService<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/Auth/reset-password", request);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ResponseFromService<string>>(json, JsonOptions)!;
        }
        catch (Exception ex)
        {
            return new ResponseFromService<string>
            {
                Code = -999,
                Message = "Error al restablecer contraseña",
                Toast = "error",
                Descripcion = [ex.Message],
                Data = string.Empty
            };
        }
    }
}
