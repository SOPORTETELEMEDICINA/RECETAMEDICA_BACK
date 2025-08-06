namespace RMD.Models.Receta.Header.Responses
{
    public class HeaderCounltResponse : HeaderToListResponse
    {    
        public int IdTipoIdentificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }    
        public string Sucursal { get; set; }
        public string GrupoEmpresarial { get; set; }

        // Nuevos campos para reflejar el estado de la receta
        public int Estatus { get; set; }
        public string Descripcion { get; set; }
    }
}
