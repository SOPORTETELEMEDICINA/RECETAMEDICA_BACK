﻿namespace RMD.Models.Vidal
{
    public class ForeignProductEquivalent : VidalBaseModel
    {
        public string MarketStatus { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Vmp { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
    }
}
