using Hastane.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hastane.Models
{
    public class doktorList
    {
       public List<personel> personelListesi { get; set; }
        public List<doktor> doktorListesi { get; set; }
        public List<izin> izinListesi { get; set; }
        public List<mesajgiden> mesajgidenListesi { get; set; }
        public List<hastane> hastaneListesi { get; set; }
        public List<klinik> klinikListesi { get; set; }
        public List<ilac> ilacListesi { get; set; }
    }
}