namespace RMD.Interface.Security
{
    public interface IConfiguracionGlobalService
    {
        Task<string> GetValorAsync(string clave);
        Task<int?> GetValorIntAsync(string clave);
        Task<bool?> GetValorBoolAsync(string clave);
        Task ReloadIfUpdatedAsync(); // Por si lo quieres invocar manual
    }

}
