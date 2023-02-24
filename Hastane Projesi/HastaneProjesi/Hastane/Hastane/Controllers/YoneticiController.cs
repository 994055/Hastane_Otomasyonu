using Hastane.Models;
using Hastane.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hastane.Controllers
{
    public class YoneticiController : Controller
    {
        HastaneEntities2 hs = new HastaneEntities2();
        // GET: Yonetici
        [HttpGet]
        [Authorize(Roles = "Y")]
        public ActionResult Index(string doktortc)
        {
            doktorList dr = new doktorList();
            dr.personelListesi = hs.personel.ToList();
            var degerler = from d in hs.doktor select d;
            if (!string.IsNullOrEmpty(doktortc))
            {
                degerler = degerler.Where(x => x.tc.Contains(doktortc));
            }
            dr.doktorListesi = degerler.ToList();
            return View(dr);
        }
        [Authorize]
        [HttpGet]
        public ActionResult DoktorEkle()
        {
            ViewModels vm = new ViewModels();

            List<SelectListItem> unvan = (from x in hs.unvan.ToList()
                                          where x.kadroid != 5
                                          select new SelectListItem
                                          {
                                              Text = x.unvan1,
                                              Value = x.kadroid.ToString()
                                          }).ToList();
            ViewBag.unvan = unvan;

            List<SelectListItem> klinik = (from x in hs.klinik.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.klinik1,
                                               Value = x.klinikid.ToString()
                                           }).ToList();
            ViewBag.klinik = klinik;
            vm.iller = new SelectList(hs.iller, "id", "sehiradi");
            vm.hastaneler = new SelectList(hs.hastane, "hastaneid", "hastaneadi");

            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public ActionResult DoktorEkle(personel p, kullanici_Yetki k, doktor d, string personelgorev)
        {
           
            if (p != null)
            {
                p.kalanizin = 8;
                p.kullanilanizin = 0;
                hs.personel.Add(p);
                hs.SaveChanges();
                var pers = hs.personel.Where(x => x.tc == p.tc).FirstOrDefault();
                if (personelgorev == "D")
                {
                    d.personelID = pers.personelid;
                    d.izindeMi = false;
                    hs.doktor.Add(d);
                    hs.SaveChanges();
                    k.kullaniciRol = "D";
                    k.klinik = true;
                    k.vezne = false;
                    k.personelid = pers.personelid;
                    hs.kullanici_Yetki.Add(k);
                    hs.SaveChanges();
                }                             
                return RedirectToAction("Index", "Yonetici");
            }

            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Doktorizinonay()
        {
            doktorList dr = new doktorList();
            dr.doktorListesi = hs.doktor.ToList();
            dr.personelListesi = hs.personel.ToList();
            dr.izinListesi = hs.izin.Where(x => x.onaylandimi == false).ToList();
            return View(dr);
        }
        [Authorize]
        [HttpGet]
        public ActionResult Doktorizinver(int? personelID, DateTime izinbitis, DateTime izinbas)
        {
            doktor doc = new doktor();
            izin iz = new izin();
            doc = hs.doktor.Where(x => x.personelID == personelID).FirstOrDefault();
            iz = hs.izin.Where(x => x.personelid == personelID && x.onaylandimi == false).FirstOrDefault();
            var per = hs.personel.Find(personelID);
            TimeSpan kullanilanizin = izinbitis - izinbas;
            double kullanilanizingun = kullanilanizin.TotalDays;
            int day = Convert.ToInt32(kullanilanizingun);
            if (doc != null)
            {
                iz.onaylandimi = true;
                doc.izindeMi = true;
                per.izinbitis = izinbitis;
                per.kalanizin = per.kalanizin - day;
                hs.SaveChanges();
            }
            return RedirectToAction("Doktorizinonay", "Yonetici");
        }
        [Authorize]
        public ActionResult DoktorDetay(int? personelID)
        {
            doktorList dl = new doktorList();
            dl.doktorListesi = hs.doktor.Where(x => x.personelID == personelID).ToList();
            return View(dl);
        }
        [Authorize]
        [HttpGet]
        public ActionResult DoktorGuncelle(int? personelID)
        {
            if (personelID != null)
            {
                doktorList dl = new doktorList();
                dl.doktorListesi = hs.doktor.Where(x => x.personelID == personelID).ToList();
                return View(dl);
            }
            else
            {
                return RedirectToAction("Index", "Yonetici");
            }
        }   
        [HttpPost]
        public ActionResult DoktorGuncelle(int? drID, string ad, string soyad, string tc, string ceptel, string adres, string email, string sifre, string dtarih, string cinsiyet, string sicilno, string diplomano, string mezuniyet)
        {
            var doktor = hs.doktor.Find(drID);
            var kullanici = hs.kullanici_Yetki.Where(x => x.personelid == doktor.personelID).FirstOrDefault();
            doktor.ad = ad;
            doktor.soyad = soyad;
            doktor.tc = tc;
            doktor.personel.ceptel = ceptel;
            doktor.personel.adres = adres;
            kullanici.email = email;
            kullanici.sifre = sifre;
            doktor.personel.dtarih = Convert.ToDateTime(dtarih);
            doktor.personel.cinsiyet = cinsiyet;
            doktor.personel.sicilno = sicilno;
            doktor.personel.mezuniyet = mezuniyet;
            doktor.personel.diplomano = diplomano;

            hs.SaveChanges();
            return RedirectToAction("Index", "Yonetici");
        }
        [Authorize]
        [HttpGet]
        public ActionResult HastaneEkle()
        {
            ViewModels vm = new ViewModels();
            vm.iller = new SelectList(hs.iller, "id", "sehiradi");
            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public ActionResult HastaneEkle(int? DPil, string hastaneadi)
        {
            ViewModels vm = new ViewModels();
            vm.iller = new SelectList(hs.iller, "id", "sehiradi");
            var hst = hs.hastane.Where(x => x.hastaneadi == hastaneadi && x.ilid == DPil).FirstOrDefault();
            if (DPil != null && hastaneadi != null && hst == null)
            {
                hastane hastane = new hastane()
                {
                    hastaneadi = hastaneadi,
                    ilid = DPil
                };
                hs.hastane.Add(hastane);
                hs.SaveChanges();
                return RedirectToAction("Hastaneler", "Yonetici");
            }
            else
            {
                if (DPil == null)
                {
                    ViewBag.hataliekleme = "Lütfen hastane seçiniz!";
                }
                else
                {
                    ViewBag.hataliekleme = "Kaydetmek istediğiniz klinik zaten sisteme kayıtlı!";
                }
                return View(vm);
            }

        }
        [Authorize]
        [HttpGet]
        public ActionResult PersonelGuncelle(int? personelID)
        {
            if (personelID != null)
            {
                doktorList dl = new doktorList();
                dl.personelListesi = hs.personel.Where(x => x.personelid == personelID).ToList();
                return View(dl);
            }
            else
            {
                return RedirectToAction("Index", "Yonetici");
            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult PersonelGuncelle(int? personelID, string ad, string soyad, string tc, string ceptel, string adres, string dtarih, string cinsiyet, string sicilno, string diplomano, string mezuniyet)
        {
            var personel = hs.personel.Find(personelID);
            personel.ad = ad;
            personel.soyad = soyad;
            personel.tc = tc;
            personel.ceptel = ceptel;
            personel.adres = adres;
            personel.dtarih = Convert.ToDateTime(dtarih);
            personel.cinsiyet = cinsiyet;
            personel.sicilno = sicilno;
            personel.mezuniyet = mezuniyet;
            personel.diplomano = diplomano;
            hs.SaveChanges();
            return RedirectToAction("Personeller", "Yonetici");
        }
        [Authorize]
        [HttpGet]
        public ActionResult PersonelCikar(string personeltc, string personelad)
        {
            var degerler = from d in hs.personel where d.unvan != 5 select d;
            if (!string.IsNullOrEmpty(personeltc))
            {
                degerler = degerler.Where(x => x.tc.Contains(personeltc));
            }
            if (!string.IsNullOrEmpty(personelad))
            {
                degerler = degerler.Where(x => x.ad.Contains(personelad));
            }
            var dgr = degerler.OrderBy(x => x.ad);
            return View(dgr.ToList());
        }
        [Authorize]
        [HttpGet]
        public ActionResult PersonelCikarOnay(int? personelID)
        {
            var pers = hs.personel.Find(personelID);
            var doktor = hs.doktor.Where(x => x.personelID == pers.personelid).FirstOrDefault();
            var kullanici = hs.kullanici_Yetki.Where(x => x.personelid == pers.personelid).FirstOrDefault();
            if(kullanici != null)
            {
                hs.kullanici_Yetki.Remove(kullanici);
                hs.SaveChanges();
            }
            if(doktor != null)
            {
                hs.doktor.Remove(doktor);
                hs.SaveChanges();
            }
            hs.personel.Remove(pers);
            hs.SaveChanges();

            return RedirectToAction("PersonelCikar","Yonetici");
        }
       
        [Authorize]
        public ActionResult Personeller(string personeltc, string personelad)
        {
            var degerler = from d in hs.personel where d.unvan != 5 && d.unvan != 1 select d;
            if (!string.IsNullOrEmpty(personeltc))
            {
                degerler = degerler.Where(x => x.tc.Contains(personeltc));
            }
            if (!string.IsNullOrEmpty(personelad))
            {
                degerler = degerler.Where(x => x.ad.Contains(personelad));
            }
            var dgr = degerler.OrderBy(x => x.ad);
            return View(dgr.ToList());
        }
        [Authorize]
        public ActionResult PersonelDetay(int? personelID)
        {
            doktorList dl = new doktorList();
            dl.personelListesi = hs.personel.Where(x => x.personelid == personelID).ToList();
            return View(dl);
        }
        [Authorize]
        public ActionResult Hastaneler(string hastaneAd, string sehirAd)
        {
            var degerler = from d in hs.hastane select d;
            if (!string.IsNullOrEmpty(hastaneAd))
            {
                degerler = degerler.Where(x => x.hastaneadi.Contains(hastaneAd));
            }
            if (!string.IsNullOrEmpty(sehirAd))
            {
                degerler = degerler.Where(x => x.iller.sehiradi.Contains(sehirAd));
            }
            var dgr = degerler.OrderBy(x => x.iller.sehiradi);
            return View(dgr.ToList());

        }
        [Authorize]
        [HttpGet]
        public ActionResult HastaneGuncelle(int? hastaneID)
        {
            doktorList dl = new doktorList();
            TempData["HastaneID"] = hastaneID;
            dl.hastaneListesi = hs.hastane.Where(x => x.hastaneid == hastaneID).ToList();
            return View(dl);
        }
        [Authorize]
        [HttpPost]
        public ActionResult HastaneGuncelle(string hastaneadi, string sehiradi)
        {
            int hastaneid = Convert.ToInt32(TempData["HastaneID"]);
            var hastane = hs.hastane.Find(hastaneid);
            hastane.hastaneadi = hastaneadi;
            hastane.iller.sehiradi = sehiradi;
            hs.SaveChanges();
            return RedirectToAction("Hastaneler", "Yonetici");
        }
        [Authorize]
        [HttpGet]
        public ActionResult HastaneSil(int? hastaneID)
        {
            hastane hastane = hs.hastane.Find(hastaneID);
            hs.hastane.Remove(hastane);
            hs.SaveChanges();
            return RedirectToAction("Hastaneler", "Yonetici");
        }
        [Authorize]
        [HttpGet]
        public ActionResult KlinikEkle()
        {
            ViewModels vm = new ViewModels();
            vm.iller = new SelectList(hs.iller, "id", "sehiradi");
            vm.hastaneler = new SelectList(hs.hastane, "hastaneid", "hastaneadi");
            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public ActionResult KlinikEkle(int? DPil, int? DPhastane, string klinik, string yataksayisi)
        {
            ViewModels vm = new ViewModels();
            vm.iller = new SelectList(hs.iller, "id", "sehiradi");
            vm.hastaneler = new SelectList(hs.hastane, "hastaneid", "hastaneadi");
            var kln = hs.klinik.Where(x => x.klinik1 == klinik && x.hastane == DPhastane).FirstOrDefault();
            if (DPil != null && klinik != null && DPhastane != null && kln == null)
            {
                klinik yeniklinik = new klinik()
                {
                    klinik1 = klinik,
                    hastane = DPhastane,
                    yataksayi = Convert.ToInt32(yataksayisi)
                };
                hs.klinik.Add(yeniklinik);
                hs.SaveChanges();
                return RedirectToAction("Klinikler", "Yonetici");
            }
            else
            {
                if (DPil == null || DPhastane == null)
                {
                    ViewBag.hataliekleme = "Lütfen şehir ve hastane bilgilerini eksiksiz giriniz!";
                }
                else
                {
                    ViewBag.hataliekleme = "Kaydetmek istediğiniz klinik zaten sisteme kayıtlı!";
                }

                return View(vm);
            }
        }
        [Authorize]
        public ActionResult Klinikler(string hastaneAd, string klinik)
        {
            var degerler = from d in hs.klinik select d;
            if (!string.IsNullOrEmpty(klinik))
            {
                degerler = degerler.Where(x => x.klinik1.Contains(klinik));
            }
            if (!string.IsNullOrEmpty(hastaneAd))
            {
                degerler = degerler.Where(x => x.hastane1.hastaneadi.Contains(hastaneAd));
            }
            var dgr = degerler.OrderBy(x => x.hastane1.hastaneadi);
            return View(dgr.ToList());
        }
        [Authorize]
        [HttpGet]
        public ActionResult KlinikGuncelle(int? klinikID)
        {
            doktorList dl = new doktorList();
            TempData["klinikID"] = klinikID;
            dl.klinikListesi = hs.klinik.Where(x => x.klinikid == klinikID).ToList();
            return View(dl);
        }
        [Authorize]
        [HttpPost]
        public ActionResult KlinikGuncelle(string klinikadi)
        {
            int klinikid = Convert.ToInt32(TempData["klinikID"]);
            var yeniklinik = hs.klinik.Find(klinikid);
            yeniklinik.klinik1 = klinikadi;
            hs.SaveChanges();
            return RedirectToAction("Klinikler", "Yonetici");
        }
        [Authorize]

        [HttpGet]
        public ActionResult KlinikSil(int? klinikID)
        {
            klinik klinik = hs.klinik.Find(klinikID);
            hs.klinik.Remove(klinik);
            hs.SaveChanges();
            return RedirectToAction("Klinikler", "Yonetici");
        }
        [Authorize]
        public ActionResult Ilaclar(string ilacad, string ilacbarkod)
        {
            var degerler = from d in hs.ilac select d;
            if (!string.IsNullOrEmpty(ilacad))
            {
                degerler = degerler.Where(x => x.ilacad.Contains(ilacad));
            }
            if (!string.IsNullOrEmpty(ilacbarkod))
            {
                degerler = degerler.Where(x => x.ilacbarkod.Contains(ilacbarkod));
            }
            var dgr = degerler.OrderBy(x => x.ilacad);
            return View(dgr.ToList());
        }
        [Authorize]
        [HttpGet]
        public ActionResult ilacGuncelle(int? ilacID)
        {
            doktorList dl = new doktorList();
            TempData["ilacID"] = ilacID;
            dl.ilacListesi = hs.ilac.Where(x => x.ilacid == ilacID).ToList();
            return View(dl);
        }
        [Authorize]
        [HttpPost]
        public ActionResult ilacGuncelle(string ilacadi, string ilacbarkod)
        {
            int ilacid = Convert.ToInt32(TempData["ilacID"]);
            var yeniilac = hs.ilac.Find(ilacid);
            yeniilac.ilacad = ilacadi;
            yeniilac.ilacbarkod = ilacbarkod;
            hs.SaveChanges();
            return RedirectToAction("Ilaclar", "Yonetici");
        }
        [Authorize]
        [HttpGet]
        public ActionResult IlacSil(int? ilacID)
        {
            ilac ilac = hs.ilac.Find(ilacID);
            hs.ilac.Remove(ilac);
            hs.SaveChanges();
            return RedirectToAction("Ilaclar", "Yonetici");
        }
        [Authorize]
        [HttpGet]
        public ActionResult ilacEkle()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult ilacEkle(string ilacad, string ilacbarkod)
        {
            var ilc = hs.ilac.Where(x => x.ilacad == ilacad && x.ilacbarkod == ilacbarkod).FirstOrDefault();
            if (ilacad != "" && ilacbarkod != "")
            {
                ilac yeniilac = new ilac()
                {
                    ilacad = ilacad,
                    ilacbarkod = ilacbarkod
                };
                hs.ilac.Add(yeniilac);
                hs.SaveChanges();
                return RedirectToAction("Ilaclar", "Yonetici");
            }
            else
            {
                ViewBag.hataliekleme = "Kaydetmek istediğiniz ilaç zaten sisteme kayıtlı!";
                return View();
            }
        }

   
        public JsonResult hastanegetir(int p)
        {
            var hastaneler = (from x in hs.hastane
                              join y in hs.iller on x.iller.id equals y.id
                              where x.iller.id == p
                              select new
                              {
                                  Text = x.hastaneadi,
                                  Value = x.hastaneid.ToString()
                              }).ToList();
            var hastaneyok = (from x in hs.hata
                              where x.hataid == 1
                              select new
                              {
                                  Text = x.hatamesaji,
                                  Value = x.hataid
                              }).ToList();
            if (hastaneler.Count != 0)
            {
                return Json(hastaneler, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(hastaneyok, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult klinikgetir(int p)
        {
            var klinikler = (from x in hs.klinik
                             join y in hs.hastane on x.hastane1.hastaneid equals y.hastaneid
                             where x.hastane1.hastaneid == p
                             select new
                             {
                                 Text = x.klinik1,
                                 Value = x.klinikid.ToString()
                             }).ToList();
            var klinikyok = (from x in hs.hata
                             where x.hataid == 2
                             select new
                             {
                                 Text = x.hatamesaji,
                                 Value = x.hataid
                             }).ToList();
            if (klinikler.Count != 0)
            {
                return Json(klinikler, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(klinikyok, JsonRequestBehavior.AllowGet);
            }
        }
    }
}