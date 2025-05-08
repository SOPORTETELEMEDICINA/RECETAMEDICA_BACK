using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Interface.Usuarios
{
    public interface ICatGrupoEmpresarialService
    {
        Task<ResponseFromService<IEnumerable<CatGrupoEmpresarial>>> GetAllGrupoEmpresarialAsync();
        Task<ResponseFromService<CatGrupoEmpresarial>> GetGrupoEmpresarialByIdAsync(Guid id);
        Task<ResponseFromService<string>> CreateGrupoEmpresarialAsync(CatGrupoEmpresarial grupoEmpresarial);
        Task<ResponseFromService<string>> UpdateGrupoEmpresarialAsync(CatGrupoEmpresarial grupoEmpresarial);
    }
}
