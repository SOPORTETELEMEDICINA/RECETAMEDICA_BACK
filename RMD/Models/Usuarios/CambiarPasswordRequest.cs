namespace RMD.Models.Usuarios
{
    public class CambiarPasswordRequest
    {
        public string NuevaPassword { get; set; }
        public string ConfirmacionPassword { get; set; }
    }

}
