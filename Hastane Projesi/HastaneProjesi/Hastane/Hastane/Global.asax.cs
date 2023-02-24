using Hastane.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hastane.Models.Entity;
using System.Web.Optimization;
using System.Web.Routing;

namespace Hastane
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            HastaneEntities2 hs = new HastaneEntities2();
            var doktor = hs.doktor.Where(x => x.izindeMi == true).ToList();
            foreach (var dr in doktor)
            {
                var pers = hs.personel.Find(dr.personelID);
                if (DateTime.Today > pers.izinbitis)
                {
                    dr.izindeMi = false;
                    hs.SaveChanges();
                }
            }

            var muayene = hs.muayene.Where(x => x.yatisbitistarihi < DateTime.Today && x.yatis==true && x.cikisonay==false).ToList();
            foreach (var my in muayene)
            {
                my.cikisonay = true;
                int klinikid = Convert.ToInt32(my.poliklinik);
                var klinik = hs.klinik.Find(klinikid);
                klinik.yataksayi += 1;
                hs.SaveChanges();
            }

            Database.SetInitializer<Hastane.Models.DatabaseContext>(null);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
