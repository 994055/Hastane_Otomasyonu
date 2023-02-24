using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Eczane.Models.Entity;

namespace Eczane.Models
{
    public class stokList
    {
        public List<ilac> ilacListesi { get; set; }
        public List<recete> receteListesi { get; set; }
        public List<fatura> faturaListesi { get; set; }
        public IEnumerable<SelectListItem> ilac { get; set; }
    }
}