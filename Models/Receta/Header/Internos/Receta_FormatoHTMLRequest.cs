namespace RMD.Models.Receta.Header.Internos
{
    public class Receta_FormatoHTMLRequest
    {
        public int IdFormatoReceta { get; set; }
        public string Formato { get; set; }
        public string Logo { get; set; } = string.Empty;
        public string LogoSuperior { get; set; }
        public string LogoInferior { get; set; }

    }

}
