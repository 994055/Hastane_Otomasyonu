//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eczane.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class fatura
    {
        public int fatura_id { get; set; }
        public string hasta_ad { get; set; }
        public string hasta_tc { get; set; }
        public string hasta_odeme_turu { get; set; }
        public double odenen_tutar { get; set; }
        public string satilanilac { get; set; }
        public System.DateTime fatura_traihi { get; set; }
        public Nullable<int> faturayikesenid { get; set; }
    }
}