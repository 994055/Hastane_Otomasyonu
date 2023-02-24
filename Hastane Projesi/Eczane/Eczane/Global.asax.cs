using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Eczane.Models.Entity;
using Eczane.ServiceReference1;

namespace Eczane
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //eczaneEntities ec = new eczaneEntities();
            //WebService1SoapClient servis = new WebService1SoapClient();
            //List<receteTut> receteListesi = new List<receteTut>();
            //var receteList = servis.GetRecete();
            //foreach (var rct in receteList)
            //{
            //    recete dgr = new recete()
            //    {
            //        hastaad = rct.hastaad,
            //        hastatc = rct.hastatc,
            //        ilacbarkod = rct.ilackodu,
            //        recetekodu = rct.recetekodu,
            //        recetetarih = (DateTime)rct.recetetarih,
            //        ilacadet = rct.ilacadet,
            //        ilacVerildiMi = 0
            //    };
            //    var kontrol = ec.recete.Where(x => x.recetekodu == dgr.recetekodu).FirstOrDefault();
            //    if (kontrol == null)
            //    {
            //        ec.recete.Add(dgr);
            //        ec.SaveChanges();
            //    }
            //}





            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public class receteTut
        {
            public string hastaad { get; set; }
            public string hastatc { get; set; }
            public string ilacbarkod { get; set; }
            public string recetekodu { get; set; }
            public string ilacadet { get; set; }
            public System.DateTime recetetarih { get; set; }
            public Nullable<sbyte> ilacVerildiMi { get; set; }
        }
    }
}
