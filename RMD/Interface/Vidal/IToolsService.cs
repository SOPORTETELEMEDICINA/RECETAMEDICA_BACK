using RMD.Shared.Models.Vidal.Tools;
using RMD.Shared.Models.Vidal.Tools.Alertas;

namespace RMD.Interface.Vidal
{
    public interface IToolsService
    {
        Task<ResponseFromService<IEnumerable<DocumentOption>>> GetDocumentListAsync(FichaHtmlRequest peticion);
        Task<ResponseFromService<string>> GetFichaHtmlAsync(string path);
        //Task<ResponseFromService<JsonElement>> GetTipoAlertaHtmlAsync(int type);

        Task<ResponseFromService<string>> GetContenidoDesdeRutaAsync(string ruta);

        Task<ResponseFromService<List<AlertaJsonHtmlModel>>> GetTipoAlertaJsonAsync(int type);
        //Task<ResponseFromService<JsonElement>> GetTipoAlertaJsonAsync(int type);
        //Task<ResponseFromService<string>> GetTipoAlertaHtmlAsync(int type);
        Task<ResponseFromService<IEnumerable<AlertaOption>>> GetTiposDeAlertasAsync();
        Task<ResponseFromService<string>> GetNoticiaHtmlById(string type, int idNoticia);
        Task<ResponseFromService<string>> GetNoticiaRelatedHtmlById(int idNoticia);
        Task GuardarTodasLasAlertasDesdeVidalAsync();
    }
}
