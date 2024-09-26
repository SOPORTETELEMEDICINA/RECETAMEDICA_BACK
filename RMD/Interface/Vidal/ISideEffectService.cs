using RMD.Models.Vidal.BySideEffect;

namespace RMD.Interface.Vidal
{
    public interface ISideEffectService
    {
        Task<SideEffect> GetSideEffectByIdAsync(int id);
    }
}
