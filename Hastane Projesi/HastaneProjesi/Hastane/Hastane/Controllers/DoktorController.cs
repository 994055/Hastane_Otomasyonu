using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Hastane.Models;
using Hastane.Models.Entity;
using System.Data.SqlTypes;

namespace Hastane.Controllers
{
    public class DoktorController : Controller
    {
        HastaneEntities2 hs = new HastaneEntities2();
        // GET: Doktor
        public ActionResult Index()
        {
            int klinikid = Convert.ToInt32(Session["klinikID"]);
            var klinik = hs.klinik.Where(x => x.klinikid == klinikid).FirstOrDefault();
            if (klinik != null)
            {
                Session["hastanead"] = klinik.hastane1.hastaneadi;
            }

            return View();
        }
        [HttpGet]
        public ActionResult mesaj(int? kimeid)
        {
            doktorList dl = new doktorList();

            int klinikid = Convert.ToInt32(Session["klinikID"]);
            int drid = Convert.ToInt32(Session["DoktorID"]);
            dl.doktorListesi = hs.doktor.Where(x => x.klinik == klinikid && x.drid != drid).ToList();
            if (kimeid == null)
            {
                var doktor = hs.doktor.Where(x => x.klinik == klinikid && x.drid != drid).FirstOrDefault();
                kimeid = doktor.drid;
            }

            dl.mesajgidenListesi = hs.mesajgiden.Where(x => x.kimdenid == drid && x.kimeid == kimeid).ToList();
            return View(dl);
        }

        [HttpPost]
        public ActionResult MesajGonder(string mesaj, int? kimeid)
        {
            int drid = Convert.ToInt32(Session["DoktorID"]);
            int sayac = 1;
            var ilkMesajMi = hs.mesajgiden.Where(x => x.kimdenid == drid && x.kimeid == kimeid).ToList();
            if (ilkMesajMi.Count == 0)
            {
                mesajgiden gidenmesaj = new mesajgiden()
                {
                    kimdenid = kimeid,
                    kimeid = drid,
                    mesaj = ".",
                    cevaplananmesaj = 0
                };
                sayac -= 1;
                hs.mesajgiden.Add(gidenmesaj);
                hs.SaveChanges();
            }
            var mesajlar = hs.mesajgiden.Where(x => x.kimeid == drid && x.kimdenid == kimeid).ToList();
            int cevaplananmesajid = 0;
            foreach (var msj in mesajlar)
            {
                cevaplananmesajid = msj.gidenmesajid;
            }
            if (sayac == 0)
            {
                var ilkmesaj = hs.mesajgiden.Where(x => x.mesaj == "." && x.kimdenid == kimeid && x.kimeid == drid).FirstOrDefault();
                cevaplananmesajid = ilkmesaj.gidenmesajid;
            }
            if (!mesaj.Equals(""))
            {
                mesajgiden gidenmesaj = new mesajgiden
                {
                    kimdenid = drid,
                    kimeid = kimeid,
                    tarih = DateTime.Now,
                    mesaj = mesaj,
                    cevaplananmesaj = cevaplananmesajid
                };
                hs.mesajgiden.Add(gidenmesaj);
                hs.SaveChanges();
            }

            return RedirectToAction("mesaj", kimeid);
        }
        [HttpGet]
        public ActionResult HastaRandevuListele()
        {
            randevuList rl = new randevuList();
            randevu rnd = new randevu();
            int drID = Convert.ToInt32(Session["DoktorID"]);
            DateTime tarih = DateTime.Now.AddHours(10);
            Session["hatalitarih"] = "false";
            rl.randevuListesi = hs.randevu.Where(x => x.doktorid == drID && 
            x.tarih >= DateTime.Today && x.tarih <= DateTime.Today).OrderBy(x => x.saat1.saatzaman).ToList();
            if (rl.randevuListesi.Count == 0)
            {
                ViewBag.randevuYok = "Randevu Alan Hasta Yok";
            }
            return View(rl);
        }
        [HttpGet]
        public ActionResult HastaDetay(int? hastaID)
        {
            randevuList rl = new randevuList();
            if (hastaID != null)
            {
                rl.hastaListesi = hs.hasta.Where(x => x.hastaid == hastaID).ToList();
            }
            return View(rl);
        }
        [HttpGet]
        public ActionResult Doktorizin()
        {
            List<SelectListItem> izintur = (from x in hs.izintur.ToList()
                                            select new SelectListItem
                                            {
                                                Text = x.tur,
                                                Value = x.turid.ToString()
                                            }).ToList();
            ViewBag.izin = izintur;
            return View();
        }
        [HttpPost]
        public ActionResult Doktorizin(string izindekiadres, int izinturu, DateTime izinbaslangic, DateTime izinbitis)
        {
            HastaneEntities2 hs = new HastaneEntities2();
            int persid = Convert.ToInt32(Session["PersonelID"]);
            List<int> sayilar = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            ViewBag.sayi = sayilar;
            izin iz = new izin
            {
                izinturu = izinturu,
                izinbaslangic = izinbaslangic,
                izinbitis = izinbitis,
                personelid = Convert.ToInt32(Session["PersonelID"]),
                izindekiadres = izindekiadres,
                onaylandimi = false
            };

            hs.izin.Add(iz);
            hs.SaveChanges();
            return RedirectToAction("Index", "Doktor");
        }
        [HttpGet]
        public ActionResult Hastamuayene(int? hastaID, int? randevuID, int? kacAdet,int? yatisonay)
        {
            randevuList rl = new randevuList();
            rl.randevuListesi = hs.randevu.ToList();
            rl.ilac = new SelectList(hs.ilac, "ilacid", "ilacad");
            var hasta = hs.hasta.Find(hastaID);
            var randevu = hs.randevu.Find(randevuID);
            ViewBag.hastid = hastaID;
            ViewBag.randevuid = randevuID;
            ViewBag.kacadet = kacAdet;
            ViewBag.yatisonay = yatisonay;
            TempData["Hastaid"] = hastaID;
            TempData["Randevuid"] = randevuID;
        
            List<SelectListItem> unvan = (from x in hs.unvan.ToList()
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


            rl.hastaListesi = hs.hasta.Where(x => x.hastaid == hastaID).ToList();
            return View(rl);
        }
        [HttpPost]
        public ActionResult Hastamuayene(string teshis, string yatisonay, FormCollection frm, string ilac1, string ilac2, string ilac3, string ilac4, string ilac5, int? ilacadet1, int? ilacadet2, int? ilacadet3, int? ilacadet4, int? ilacadet5, DateTime yatisbaslangic, DateTime yatisson)
        {
            randevu rnd = hs.randevu.Find(Convert.ToInt32(TempData["Randevuid"]));
            rnd.geldimi = true;
            hs.SaveChanges();
            int doktorid = Convert.ToInt32(Session["DoktorID"]);
            int personelid = Convert.ToInt32(Session["PersonelID"]);
            var kullanici = hs.kullanici_Yetki.Where(x => x.personelid == personelid).FirstOrDefault();
            var dr = hs.doktor.Find(doktorid);
            var klinik = hs.klinik.Where(x => x.klinikid == dr.klinik).FirstOrDefault();
            string ilacisim = "";
            string ilackodu = "";
            string ilacadet = "";
            if (ilac1 != null)
            {
                var ilacad = hs.ilac.Find(Convert.ToInt32(ilac1));
                ilacisim += " " + ilacad.ilacad + " " + ilacadet1 + "-Adet";
                ilackodu += ilacad.ilacbarkod;
                ilacadet += ilacadet1;
            }
            if (ilac2 != null)
            {
                var ilacad = hs.ilac.Find(Convert.ToInt32(ilac2));
                ilacisim += " " + ilacad.ilacad + " " + ilacadet2 + "-Adet";
                ilackodu += "/" + ilacad.ilacbarkod;
                ilacadet += "/" + ilacadet2;
            }
            if (ilac3 != null)
            {
                var ilacad = hs.ilac.Find(Convert.ToInt32(ilac3));
                ilacisim += " " + ilacad.ilacad + " " + ilacadet3 + "-Adet";
                ilackodu += "/" + ilacad.ilacbarkod;
                ilacadet += "/" + ilacadet3;
            }
            if (ilac4 != null)
            {
                var ilacad = hs.ilac.Find(Convert.ToInt32(ilac4));
                ilacisim += " " + ilacad.ilacad + " " + ilacadet4 + "-Adet";
                ilackodu += "/" + ilacad.ilacbarkod;
                ilacadet += "/" + ilacadet4;
            }
            if (ilac5 != null)
            {
                var ilacad = hs.ilac.Find(Convert.ToInt32(ilac5));
                ilacisim += " " + ilacad.ilacad + " " + ilacadet5 + "-Adet";
                ilackodu += "/" + ilacad.ilacbarkod;
                ilacadet += "/" + ilacadet5;
            }
            else if (ilac1 == null && ilac2 == null && ilac3 == null && ilac4 == null && ilac5 == null)
            {
                ilacisim = "İlaç verilmedi";
            }
            ilacisim = ilacisim.Trim();
            int hastaID = Convert.ToInt32(TempData["Hastaid"]);
            var hasta = hs.hasta.Find(hastaID);
            bool yatiskarar;
            bool onay=true;
            SqlDateTime yatisonaytarih = SqlDateTime.Null;
            SqlDateTime yatisbitis = SqlDateTime.Null;
            if (yatisonay == "1")
            {
                yatiskarar = true;
                if (yatisson > DateTime.Now && yatisbaslangic >= DateTime.Today)
                {
                    onay = false;
                    yatisonaytarih = DateTime.Now;
                    yatisbitis = yatisson;
                }
                else
                {
                    Session["hatalitarih"]= "true";
                    return RedirectToAction("HastaRandevuListele","Doktor");
                }
                klinik.yataksayi -= 1;
            }
            else
            {
                yatiskarar = false;
            }
            DateTime tarih = (DateTime)rnd.tarih;
            muayene m = new muayene()
            {
                muayenebaslangici = rnd.tarih,
                muayenebitisi = DateTime.Now,
                muayenetarih = tarih,
                teshis = teshis,
                randevusaat = rnd.saat1.saatzaman,
                hastaid = hastaID,
                poliklinik = dr.klinik,
                yatis = yatiskarar,
                muayeneonay = true,
                kullanici = dr.personelID,
                yatistarihi = (DateTime)yatisonaytarih,
                yatisbitistarihi = (DateTime)yatisbitis,
                randevuid = rnd.randevuid,
                cikisonay=onay
            };
            hs.muayene.Add(m);
            hs.SaveChanges();
            var muayene = hs.muayene.Where(x => x.hastaid == hastaID && x.randevuid == rnd.randevuid).FirstOrDefault();
            islemler islem = new islemler()
            {
                muayeneid = m.muayeneid,
                islemtarihi = DateTime.Now,
                islemiyapan = dr.drid,
                kullanici = kullanici.kullaniciid,
                yatis = yatiskarar,
                ilac = ilacisim,
                hastaadisoyadi = hasta.ad + " " + hasta.soyad,
                hastatc = hasta.tc
            };
            rnd.geldimi = false;
            hs.islemler.Add(islem);
            hs.SaveChanges();
            if (ilackodu != "")
            {
                recete rct = new recete()
                {
                    hastaad = hasta.ad + " " + hasta.soyad,
                    hastatc = hasta.tc,
                    ilackodu = ilackodu,
                    recetekodu = hasta.ad.Substring(0, 2) + "" + hasta.tc.Substring(0, 3) + "" + rnd.randevuid + "" + dr.drid,
                    recetetarih = DateTime.Now,
                    ilacadet = ilacadet
                };
                hs.recete.Add(rct);
                hs.SaveChanges();
            }

            return RedirectToAction("HastaRandevuListele", "Doktor");
        }
        [HttpGet]
        public ActionResult Hastalarim(string hastaad, string hastatc)
        {
            int personelid = Convert.ToInt32(Session["PersonelID"]);

            var degerler = from d in hs.muayene where d.kullanici == personelid select d;
            if (!string.IsNullOrEmpty(hastaad))
            {
                degerler = degerler.Where(x => x.hasta.ad.Contains(hastaad));
            }
            if (!string.IsNullOrEmpty(hastatc))
            {
                degerler = degerler.Where(x => x.hasta.tc.Contains(hastatc));
            }
            var dgr = degerler.OrderBy(x => x.muayenetarih).ToList();
            return View(dgr.ToList());
        }
        [HttpGet]
        public ActionResult HastaMuayeneDetay(int? muayeneID)
        {
            randevuList rnd = new randevuList();
            rnd.muayeneListesi = hs.muayene.Where(x => x.muayeneid == muayeneID).ToList();
            return View(rnd);
        }
        public ActionResult Cikis()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}