using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Eczane.Models.Entity;
using Eczane.Models;
using System.Data.Entity.Validation;
using System.Data.SqlTypes;

namespace Eczane.Controllers
{
    public class YoneticiController : Controller
    {
        eczaneEntities ec = new eczaneEntities();
        [HttpGet]
        [Authorize]
        public ActionResult PersonelListele(string personelad,string personeltc)
        {
            var degerler = from d in ec.personel select d;
            if (!string.IsNullOrEmpty(personelad))
            {
                degerler = degerler.Where(x => x.ad.Contains(personelad));
            }
            if (!string.IsNullOrEmpty(personeltc))
            {
                degerler = degerler.Where(x => x.tc.Contains(personeltc));
            }
            var dgr = degerler.OrderBy(x => x.ad);
            return View(dgr.ToList());
        }
        [HttpGet]
        [Authorize]
        public ActionResult PersonelDetay(int? personelID)
        {
            personelList pl = new personelList();
            pl.personelListesi = ec.personel.Where(x => x.personelid == personelID).ToList();
            return View(pl);
        }
        [HttpGet]
        [Authorize]
        public ActionResult PersonelEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PersonelEkle(string ad, string soyad, string tc, DateTime dtarih, string diplomano, string mezuniyet, string cinsiyet, string ceptel, string adres, string email, string sifre, string sifretekrari)
        {

            if (sifre == sifretekrari)
            {
                string tarih = "1900-01-01";

                personel prs = new personel()
                {
                    ad = ad,
                    soyad = soyad,
                    tc = tc,
                    dtarih = dtarih,
                    adres = adres,
                    cinsiyet = cinsiyet,
                    ceptel = ceptel,
                    diplomano = diplomano,
                    mezuniyet = mezuniyet,
                    izinbitis = Convert.ToDateTime(tarih),
                    kullanilanizin = 0,
                    kalanizin = 8
                };
                ec.personel.Add(prs);
                ec.SaveChanges();
                var personel = ec.personel.Where(x => x.tc == tc).FirstOrDefault();
                kullanici_yetki ky = new kullanici_yetki()
                {
                    personel_id = personel.personelid,
                    kullanici_ad = ad,
                    kullanici_soyad = soyad,
                    kullanicirol = "E",
                    kullanici_sifre = sifre,
                    kullanici_email = email
                };
                ec.kullanici_yetki.Add(ky);
                ec.SaveChanges();
                return RedirectToAction("PersonelListele", "Yonetici");
            }
            else
            {
                ViewBag.hata = "Şifreler birbirinden farklı!";
                return View();
            }


        }
        [HttpGet]
        [Authorize]
        public ActionResult PersonelGuncelle(int? personelID)
        {
            if (personelID != null)
            {
                personelList pl = new personelList();
                pl.personelListesi = ec.personel.Where(x => x.personelid == personelID).ToList();
                return View(pl);
            }
            else
            {
                return RedirectToAction("PersonelListele", "Yonetici");
            }
        }
        [HttpPost]
        public ActionResult PersonelGuncelle(personel p,int? personelID,string email,string sifre)
        {
            if (p != null && email !=null && sifre !=null)
            {
                var pers = ec.personel.Find(personelID);
                pers.ad = p.ad;
                pers.soyad = p.soyad;
                pers.tc = p.tc;
                pers.mezuniyet = p.mezuniyet;
                pers.ceptel = p.ceptel;
                pers.adres = p.adres;
                pers.cinsiyet = p.cinsiyet;
                pers.diplomano = p.diplomano;
                ec.SaveChanges();
                var kullanici = ec.kullanici_yetki.Where(x => x.personel_id == personelID).FirstOrDefault();
                kullanici.kullanici_email = email;
                kullanici.kullanici_sifre = sifre;
                return RedirectToAction("PersonelListele", "Yonetici");
            }
            else
            {
                ViewBag.Hata = "Hatalı bilgi girişi!";
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public ActionResult PersonelCikar(int? personelID)
        {
            if(personelID != null)
            {
                var pers = ec.personel.Find(personelID);
                var kullanici = ec.kullanici_yetki.Where(x => x.personel_id == personelID).FirstOrDefault();
                ec.kullanici_yetki.Remove(kullanici);
                ec.SaveChanges();
                ec.personel.Remove(pers);
                ec.SaveChanges();          
            }
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult ilacStok(string ilac_barkod, string ilac_ad)
        {
            var degerler = from d in ec.ilac select d;
            if (!string.IsNullOrEmpty(ilac_barkod))
            {
                degerler = degerler.Where(x => x.ilac_barkod.Contains(ilac_barkod));
            }
            if (!string.IsNullOrEmpty(ilac_ad))
            {
                degerler = degerler.Where(x => x.ilac_ad.Contains(ilac_ad));
            }
            var dgr = degerler.OrderBy(x => x.ilac_ad);
            return View(dgr.ToList());
        }
        [HttpGet]
        [Authorize]
        public ActionResult ilacStokEkle(int ilacID)
        {
            stokList sl = new stokList();
            sl.ilacListesi = ec.ilac.Where(x => x.ilac_id == ilacID).ToList();
            return View(sl);
        }
        [HttpPost]
        public ActionResult ilacStokEkle(ilac p)
        {
            var ilac = ec.ilac.Where(x => x.ilac_barkod == p.ilac_barkod).FirstOrDefault();
            ilac.ilacadet += p.ilacadet;
            ec.SaveChanges();
            return RedirectToAction("ilacStok", "Yonetici");
        }
        [HttpGet]
        [Authorize]
        public ActionResult ilacEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ilacEkle(ilac p)
        {
            if (p != null)
            {
                ec.ilac.Add(p);
                ec.SaveChanges();
                return RedirectToAction("ilacStok");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        [Authorize]
        public ActionResult ilacListele(string ilac_barkod, string ilac_ad)
        {
            var degerler = from d in ec.ilac select d;
            if (!string.IsNullOrEmpty(ilac_barkod))
            {
                degerler = degerler.Where(x => x.ilac_barkod.Contains(ilac_barkod));
            }
            if (!string.IsNullOrEmpty(ilac_ad))
            {
                degerler = degerler.Where(x => x.ilac_ad.Contains(ilac_ad));
            }
            var dgr = degerler.OrderBy(x => x.ilac_ad);
            return View(dgr.ToList());
        }
        [HttpGet]
        [Authorize]
        public ActionResult ilacGuncelle(int? ilacID)
        {
            stokList sl = new stokList();
            sl.ilacListesi = ec.ilac.Where(x => x.ilac_id == ilacID).ToList();
            return View(sl);
        }
        [HttpPost]
        public ActionResult ilacGuncelle(string ilac_ad,string ilac_barkod,double? ilacfiyat, int? ilacID)
        {
            
            if(ilac_ad !=null&&ilac_barkod !=null && ilacfiyat != null)
            {
                var ilac = ec.ilac.Find(ilacID);
                ilac.ilac_ad = ilac_ad;
                ilac.ilac_barkod = ilac_barkod;
                ilac.ilacfiyat = ilacfiyat;
                ec.SaveChanges();
                return RedirectToAction("ilacListele", "Yonetici");
            }
            else
            {
                ViewBag.Hata = "Hatalı bilgi girişi";
                stokList sl = new stokList();
                sl.ilacListesi = ec.ilac.Where(x => x.ilac_id == ilacID).ToList();
                return View(sl);
            }                 
        }
        [HttpGet]
        [Authorize]
        public ActionResult ilacSil(int? ilacID)
        {
            var ilac = ec.ilac.Find(ilacID);
            ec.ilac.Remove(ilac);
            ec.SaveChanges();
            return RedirectToAction("ilacListele", "Yonetici");
        }

    }
}