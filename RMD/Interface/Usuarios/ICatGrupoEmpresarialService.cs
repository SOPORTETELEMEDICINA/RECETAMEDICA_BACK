using RMD.Models.Usuarios;

namespace RMD.Interface.Usuarios
{
    public interface ICatGrupoEmpresarialService
    {
        Task<IEnumerable<CatGrupoEmpresarial>> GetAllGrupoEmpresarial();
        Task<CatGrupoEmpresarial> GetGrupoEmpresarialById(Guid id);
        Task<string> CreateGrupoEmpresarial(CatGrupoEmpresarial grupoEmpresarial);
        Task<string> UpdateGrupoEmpresarial(CatGrupoEmpresarial grupoEmpresarial);
    }
}
