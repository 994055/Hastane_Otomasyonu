using Hastane.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hastane.Models
{
    public class randevuList
    {
        public List<randevu> randevuListesi { get; set; }
        public List<hasta> hastaListesi { get; set; }
        public List<izin> izinListesi { get; set; }
        public List<muayene> muayeneListesi { get; set; }
        public List<doktor> doktorListesi { get; set; }       
        public IEnumerable<SelectListItem> ilac { get; set; }
        public IEnumerable<SelectListItem> iller { get; set; }
        public IEnumerable<SelectListItem> hastaneler { get; set; }
    }
}