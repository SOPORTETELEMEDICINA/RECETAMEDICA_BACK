namespace RMD.Models.Vidal.CargaCatalogos
{
    public class VMPModelLink
    {
        public VMPModel Vmp { get; set; }
        public List<LinkModel> Links { get; set; }

        public VMPModelLink()
        {
            Links = new List<LinkModel>();
        }
    }

}
