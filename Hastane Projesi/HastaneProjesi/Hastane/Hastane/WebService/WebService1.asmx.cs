using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Hastane.Models.Entity;

namespace Hastane.WebService
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        HastaneEntities2 hs = new HastaneEntities2();


        [WebMethod]
        public List<ReceteItem> GetRecete()
        {
            var recetelist = (from m in hs.recete
                              select new ReceteItem
                              {
                                  ilackodu = m.ilackodu,
                                  hastaad = m.hastaad,
                                  hastatc = m.hastatc,
                                  recetekodu = m.recetekodu,
                                  recetetarih = m.recetetarih,
                                  ilacadet = m.ilacadet
                              }).ToList();
            return recetelist;
        }


        public class ReceteItem
        {
            public string ilackodu { get; set; }
            public string hastaad { get; set; }
            public string hastatc { get; set; }
            public string recetekodu { get; set; }
            public string ilacadet { get; set; }
            public Nullable<System.DateTime> recetetarih { get; set; }
        }
    }
}
