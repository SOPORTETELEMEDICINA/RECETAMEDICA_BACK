namespace RMD.Interface.Pacientes
{
    public interface IHelperPacienteServiceES
    {
        Task<ResponseFromService<string>> GenerarQRParaPacienteAsync(Guid idPaciente);
    }
}
