﻿namespace RMD.Models.Vidal.ByUCDV
{
    public class UCDVPackage
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string MarketStatus { get; set; } = string.Empty;
        public bool Otc { get; set; }
        public bool IsCeps { get; set; }
        public int DrugId { get; set; }
        public string Cip13 { get; set; } = string.Empty;
        public string ShortLabel { get; set; } = string.Empty;
        public bool Tfr { get; set; }
        public bool NarcoticPrescription { get; set; }
        public bool SafetyAlert { get; set; }
        public bool WithoutPrescription { get; set; }
        public string Product { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public string Vmp { get; set; } = string.Empty;
        public string Ucd { get; set; } = string.Empty; 
    }

}
