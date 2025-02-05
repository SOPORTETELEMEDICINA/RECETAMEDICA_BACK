namespace RMD.Models.Consulta
{
    public class RecetaPacienteModel
    {
        public Guid IdReceta { get; set; }
        public DateTime FechaReceta { get; set; }
        public Guid IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Genero { get; set; }
        public int Edad { get; set; }
    }

}
