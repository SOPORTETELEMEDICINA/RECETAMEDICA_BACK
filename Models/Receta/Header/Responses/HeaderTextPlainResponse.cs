using System.Data;
using Microsoft.Data.SqlClient;

namespace RMD.Models.Receta.Header.Responses
{
    public class HeaderTextPlainResponse
    {
        public string Folio { get; set; }
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }
        public string NombreMedico { get; set; }
        public string CedulaGeneral { get; set; }
        public string Especialidad { get; set; }
        public string CedulaEspecialidad { get; set; }
        public Guid IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public DateTime FechaNacimientoPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public decimal PacTalla { get; set; }
        public bool PacEmbarazo { get; set; }
        public string PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal PacCreatinina { get; set; }
        public string Patologias { get; set; }
        public Guid IdSucursal { get; set; }
        public string Sucursal { get; set; }
        public Guid IdGEMP { get; set; }
        public string GrupoEmpresarial { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public int Estatus { get; set; }
        public string EstatusReceta { get; set; }
        public int GenerarQR { get; set; }
        public int GenerarPDF { get; set; }
        public int GenerarEventoMedicamentoso { get; set; }
        public bool Timbrada { get; set; } = false; // Indica si la receta está timbrada
       private static bool ColumnExists(SqlDataReader reader, string columnName)
        {
            var schemaTable = reader.GetSchemaTable();
            if (schemaTable != null)
            {
                foreach (DataRow row in schemaTable.Rows)
                {
                    if (row["ColumnName"].ToString().Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }

            return false;
        }
    }
}
