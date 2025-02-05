using RMD.Models.Consulta;
using RMD.Models.PuntoVenta;

namespace RMD.Interface.PuntoVenta
{
    public interface IPuntoVentaService
    {
        Task<(RecetaModel, List<DetalleRecetaModel>, PacienteModel, MedicoModel, GrupoEmpresarialModel)>
            ObtenerRecetaAsync(Guid idReceta, Guid idMedico, DateTime fechaUltimaModificacion);
        Task SurtirMedicamentosAsync(Guid idReceta, List<Guid> detallesReceta);

        Task<(PuntoVentaRecetaModel?, List<DetalleRecetaModel>)> ConsultarRecetaPorIdAsync(int idQR);
    }
}
