using RMD.Models.Medicos;

public class PacientePorSucursalListModel
{
    public Guid IdUsuario { get; set; }
    public Guid IdPaciente { get; set; }
    public Guid IdGEMP { get; set; } // Ajustado según el SP
    public string LogoGEMP { get; set; }
    public Guid IdSucursal { get; set; } // Ajustado según el SP
    public string Nombres { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public int IdTipoIdentificacion { get; set; }
    public string TipoIdentificacion { get; set;}
    public string NumeroIdentificacion { get; set; }
    public string FechaNacimiento { get; set; }
    public int Edad { get; set; }
    public int IdEntidadNacimiento { get; set; }
    public string Genero { get; set; }
    public List<string> Alergias { get; set; } = new List<string>();
    public List<string> Molecules { get; set; } = new List<string>();
    public List<PatologiaModel> Patologias { get; set; } = new List<PatologiaModel>();
    public string Movil { get; set; }
    public string Email { get; set; }
    public string Domicilio { get; set; }
    public int? IdAsentamiento { get; set; }
    public string Asentamiento { get; set; }
    public int? IdTipoAsentamiento { get; set; }
    public string TipoAsentamiento { get; set; }
    public int? IdCP { get; set; }
    public string CodigoPostal { get; set; }
    public int? IdMunicipio { get; set; }
    public short NoMunicipio { get; set; }
    public string Municipio { get; set; }
    public int? IdCiudad { get; set; }
    public string Ciudad { get; set; }
    public int IdEntidad { get; set; }
    public string Estado { get; set; } // Ajustado según el SP
    public string Abreviatura { get; set; }
    public string Status { get; set; }
}
