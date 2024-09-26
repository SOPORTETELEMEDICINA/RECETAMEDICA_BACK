namespace RMD.Models.Vidal.ByATC
{
    public class ATCClassification
    {
        public string Id { get; set; } = string.Empty; // vidal://atc_classification/XXX
        public string VidalId { get; set; } = string.Empty; // <vidal:id>XXX</vidal:id>
        public string Name { get; set; } = string.Empty; // <vidal:name>XXX</vidal:name>
        public string Code { get; set; } = string.Empty; // <vidal:code>XXX</vidal:code>
        public string ParentLink { get; set; } = string.Empty; // Link rel="related" title="PARENT"
        public string ChildrenLink { get; set; } = string.Empty; // Link rel="related" title="CHILDREN"
        public DateTime Updated { get; set; } // <updated>XXX</updated>
        public string Category { get; set; } = string.Empty; // <category term="ATC_CLASSIFICATION"/>
        public string Author { get; set; } = string.Empty; // <author><name>VIDAL</name></author>
    }
}
