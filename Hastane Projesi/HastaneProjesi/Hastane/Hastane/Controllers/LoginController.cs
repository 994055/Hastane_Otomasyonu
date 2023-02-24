using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Hastane.Models;
using Hastane.Models.Entity;

namespace Hastane.Controllers
{
    public class LoginController : Controller
    {
        HastaneEntities2 hs = new HastaneEntities2();
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(kullanici_Yetki p)
        {
            kullanici_Yetki kullaniciGiris = hs.kullanici_Yetki.Where(x => x.email == p.email && x.sifre == p.sifre).FirstOrDefault();

            if (kullaniciGiris != null)
            {
                doktor doktorGiris = hs.doktor.Where(x => x.personelID == kullaniciGiris.personelid).FirstOrDefault();
                ViewModels vm = new ViewModels();
                FormsAuthentication.SetAuthCookie(kullaniciGiris.email, false);
                Session["PersonelEmail"] = kullaniciGiris.email.ToString();
                Session["PersonelID"] = kullaniciGiris.personelid.ToString();
                Session["klinikID"] = kullaniciGiris.klinik;                
                Session["PersonelCinsiyet"] = kullaniciGiris.cinsiyet.ToString().Trim();
                string tut = Session["PersonelCinsiyet"].ToString();
                if (doktorGiris != null)
                {
                    Session["DoktorID"] = Convert.ToInt32(doktorGiris.drid);
                }

                Session["PersonelAd"] = kullaniciGiris.ad.ToString() + " " + kullaniciGiris.soyad.ToString();



                return RedirectToAction("Index", "Doktor");
            }
            else
            {
                ViewBag.yanlisgiris = "Yanlış kullanıcı girişi!";
                return View();
            }

        }
    }
}