using Hastane.Models;
using Hastane.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Hastane.Controllers
{
    public class HastaController : Controller
    {
        // GET: Hasta
        HastaneEntities2 hs = new HastaneEntities2();

        ViewModels vm = new ViewModels();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpGet]
        public ActionResult HastaBilgiler()
        {
            randevuList rl = new randevuList();
            int hastaid = Convert.ToInt32(Session["HastaID"]);
            rl.hastaListesi = hs.hasta.Where(x => x.hastaid == hastaid).ToList();
            return View(rl);
        }
        [HttpPost]
        public ActionResult HastaBilgiler(hasta h)
        {
            int hastaid = Convert.ToInt32(Session["HastaID"]);
            var hastakisi = hs.hasta.Find(hastaid);
            hastakisi.ad = h.ad;
            hastakisi.soyad = h.soyad;
            hastakisi.tc = h.tc;
            hastakisi.dtarih = h.dtarih;
            hastakisi.cinsiyet = h.cinsiyet;
            hastakisi.adres = h.adres;
            hastakisi.babaad = h.babaad;
            hastakisi.annead = h.annead;
            hastakisi.ceptel = h.ceptel;
            hastakisi.email = h.email;
            hastakisi.sifre = h.sifre;
            hs.SaveChanges();
            return RedirectToAction("HastaBilgiler", "Hasta");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Randevu()
        {
            vm.iller = new SelectList(hs.iller, "id", "sehiradi");
            vm.hastaneler = new SelectList(hs.hastane, "hastaneid", "hastaneadi");
            vm.saat = new SelectList(hs.saat, "saatid", "saatzaman");
            List<SelectListItem> saatler = (from i in hs.saat
                                            select new SelectListItem
                                            {
                                                Text = i.saatzaman,
                                                Value = i.saatid.ToString()
                                            }).ToList();

            ViewBag.saat = saatler;
          
            return View(vm);
        }
        [HttpPost]
        public ActionResult Randevu(int? DPil, int? DPhastane, int? klinikid, DateTime tarih, int doktorid)
        {

            return View();
        }

        [HttpGet]
        public ActionResult RandevuOnay(int? DPil, int? DPhastane, int? klinikid, DateTime tarih, int doktorid)
        {
            var randevu = hs.randevu.Where(x => x.tarih == tarih && x.doktorid == doktorid).ToList();
            var saatListe = hs.saat.ToList();
            TempData["ilID"] = DPil;
            TempData["hastaneID"] = DPhastane;
            var klinik = hs.klinik.Find(klinikid);
            TempData["klinikID"] = klinikid;
            TempData["tarih"] = tarih;
            TempData["drid"] = doktorid;
            int hastaid = Convert.ToInt32(Session["HastaID"]);
            var kontrol = hs.randevu.Where(x => x.tarih == tarih && x.hastaid == hastaid && x.doktorid == doktorid).FirstOrDefault();
            if(kontrol == null)
            {
                List<SelectListItem> saatListesi = new List<SelectListItem>();

                int[] saatDizi = new int[saatListe.Count()];
                int[] randevuDizi = new int[randevu.Count()];
                int sayac = 0;
                foreach (var rnd in randevu)
                {
                    int saatid = Convert.ToInt32(rnd.saat);
                    var saat = hs.saat.Find(saatid);
                    saatListe.Remove(saat);
                }
                foreach (var saat in saatListe)
                {
                    saatListesi.Add(new SelectListItem { Text = saat.saatzaman, Value = saat.saatid.ToString() });
                }

                ViewBag.saatlistesi = saatListesi;
                return View();
            }
            else
            {
                TempData["hata"] = "Aynı Gün Aynı Doktordan Randevu alamazsınız!";
                return RedirectToAction("Randevu", "Hasta");
            }
           
        }
        [HttpPost]
        public ActionResult RandevuOnay(int? saat, int? klinikid, DateTime tarihDate, int? doktorid)
        {
            int hastaid = Convert.ToInt32(Session["HastaID"]);
            var hasta = hs.hasta.Find(hastaid);
            randevu r = new randevu()
            {
                hastaid = hastaid,
                adsyad = Session["HastaAdi"].ToString(),
                saat = saat,
                doktorid = doktorid,
                tarih = tarihDate,
                klinikid = klinikid,
                geldimi = false
            };
            if (hs.randevu.Where(x => x.doktorid == doktorid && x.tarih == tarihDate && x.saat == saat).FirstOrDefault() == null)
            {
                hs.randevu.Add(r);
                hs.SaveChanges();
                var rnd = hs.randevu.Where(x => x.hastaid == hastaid && x.tarih == tarihDate).FirstOrDefault();
                hasta.randevuID = rnd.randevuid;
                hs.SaveChanges();
                TempData["kontrol"] = "true";
            }
            else
            {
                TempData["kontrol"] = "false";

            }
            return RedirectToAction("Index", "Hasta");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Randevularim()
        {
            int hastaid = int.Parse(Session["HastaID"].ToString());
            randevuList rl = new randevuList();
            rl.randevuListesi = hs.randevu.Where(x => x.hastaid == hastaid).ToList();
            if (rl == null)
            {
                ViewBag.randevuYok = "Henüz randevunuz bulunmamaktadır.";
            }
            return View(rl);
        }
        [HttpGet]
        public ActionResult RandevularimIptal(int? randevuID)
        {
            var rnd = hs.randevu.Find(randevuID);
            if (rnd != null)
            {
                hs.randevu.Remove(rnd);
                hs.SaveChanges();
            }
            return RedirectToAction("Randevularim", "Hasta");
        }
        [HttpGet]
        public ActionResult RandevularimDetay(int? randevuID)
        {
            randevuList rl = new randevuList();
            var randevum = hs.randevu.Find(randevuID);
            int id = Convert.ToInt32(randevum.doktorid);
            rl.randevuListesi = hs.randevu.Where(x => x.randevuid == randevuID).ToList();
            rl.doktorListesi = hs.doktor.Where(x => x.drid == id).ToList();
            return View(rl);
        }
        public JsonResult hastanegetir(int p)
        {
            var hastaneler = (from x in hs.hastane
                              join y in hs.iller on x.iller.id equals y.id
                              where x.iller.id == p
                              orderby x.hastaneadi
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
                             orderby x.klinik1
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
        public JsonResult doktorgetir(int p)
        {
            var doktorlar = (from x in hs.doktor
                             join y in hs.klinik on x.klinik1.klinikid equals y.klinikid
                             orderby x.ad
                             where x.klinik1.klinikid == p && x.izindeMi == false
                             select new
                             {
                                 Text = x.ad + " " + x.soyad,
                                 Value = x.drid.ToString()
                             }).ToList();

            var doktoryok = (from x in hs.hata
                             where x.hataid == 3
                             select new
                             {
                                 Text = x.hatamesaji,
                                 Value = x.hataid
                             }).ToList();
            if (doktorlar.Count != 0)
            {
                return Json(doktorlar, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(doktoryok, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult randevugetir(int p)
        {
            var randevu = (from x in hs.saat
                           join y in hs.randevu on x.saatid equals y.saat
                           where y.doktorid == p && y.tarih == DateTime.Today && x.saatid != y.saat
                           select new
                           {
                               Text = x.saatzaman,
                               Value = x.saatid.ToString()
                           }).ToList();


            int[] indexDizi = new int[10];
            int sayac = 0;
            foreach (var saat in randevu)
            {
                indexDizi[sayac] = Convert.ToInt32(saat.Value);
                sayac++;
            }
            var saatListesi = hs.saat.ToList();
            var deneme = (from x in randevu
                          select new
                          {

                          }).ToList();
            int art = 0;
            foreach (var saat in saatListesi)
            {
                for (int i = 0; i < indexDizi.Length; i++)
                {
                    int saatid = saat.saatid;
                    if (saat.saatid != indexDizi[i])
                    {

                    }
                }
            }
            return Json(randevu, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult HastaGiris()
        {
            return View();
        }
        [HttpPost]
        public ActionResult HastaGiris(hasta P)
        {

            var hastagiris = hs.hasta.Where(x => x.email == P.email && x.sifre == P.sifre).FirstOrDefault();
            if (hastagiris != null)
            {
                FormsAuthentication.SetAuthCookie(hastagiris.email, false);
                Session["HastaEmail"] = hastagiris.email;
                Session["HastaID"] = hastagiris.hastaid;
                Session["HastaGirdi"] = "evet";
                Session["HastaAdi"] = hastagiris.ad.ToString() + " " + hastagiris.soyad.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.hata = "Hatalı kullanıcı girişi";
                return View();
            }

        }
        [HttpGet]
        public ActionResult HastaYeniKayit()
        {
            List<SelectListItem> kangrubu = (from x in hs.kanGrubu.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = x.kangrubuAdi,
                                                 Value = x.kangrubuID.ToString()
                                             }).ToList();
            ViewBag.kangrubu = kangrubu;
            return View();
        }
        [HttpPost]
        public ActionResult HastaYeniKayit(hasta p, string cinsiyet)
        {
            HastaneEntities2 hs = new HastaneEntities2();

            if (p != null)
            {
                if (hs.hasta.Where(x => x.tc == p.tc).FirstOrDefault() == null)
                {
                    p.cinsiyet = cinsiyet;
                    hs.hasta.Add(p);
                    hs.SaveChanges();
                    return RedirectToAction("HastaGiris");
                }
            }
            return View();
        }
        public ActionResult Cikis()
        {
            FormsAuthentication.SignOut();
            Session["HastaGirdi"] = "hayır";
            return RedirectToAction("HastaGiris");
        }
    }
}