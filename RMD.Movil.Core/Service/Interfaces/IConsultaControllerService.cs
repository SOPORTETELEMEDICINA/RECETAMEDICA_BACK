using RMD.Shared.Models.GlobalResponse;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IConsultaControllerService
    {
        /// <summary>
        /// Llama a POST api/Consulta/GetRelaciones con parámetros en query.
        /// </summary>
        Task<ResponseFromService<object>> GetIdsFromLinkAsync(int id, string idType, string relacionType);
    }
}
