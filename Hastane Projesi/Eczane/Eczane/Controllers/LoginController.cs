using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Eczane.Models.Entity;
using System.Web.Security;

namespace Eczane.Controllers
{
    public class LoginController : Controller
    {

      
        eczaneEntities ec = new eczaneEntities();
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(kullanici_yetki p)
        {
            kullanici_yetki kullaniciGiris = ec.kullanici_yetki.Where(x => x.kullanici_email == p.kullanici_email && x.kullanici_sifre == p.kullanici_sifre).FirstOrDefault();
        
            if (kullaniciGiris != null)
            {
                Session["PersonelAd"] = kullaniciGiris.kullanici_ad + " " + kullaniciGiris.kullanici_soyad;
                Session["PersonelCinsiyet"] = kullaniciGiris.personel.cinsiyet.ToString();
                Session["PersonelID"] = kullaniciGiris.personel.personelid.ToString();
                FormsAuthentication.SetAuthCookie(kullaniciGiris.kullanici_email, false);
                return RedirectToAction("Anasayfa", "Eczaci");
            }
            else
            {
                ViewBag.yanlisgiris = "Yanlış kullanıcı girişi!";
                return View();
            }
        
        }
       
    }
}