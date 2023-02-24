using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Eczane.Models.Entity;
using Eczane.Models;

namespace Eczane.Controllers
{
    public class EczaciController : Controller
    {

        eczaneEntities ec = new eczaneEntities();
        public ActionResult Anasayfa()
        {
            return View();
        }
        public ActionResult Cikis()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }
        [Authorize]
        [HttpGet]
        public ActionResult ilacVer(string recetekodu, string hastatc)
        {
            string tutRecete = "";
            string tutTC = "";
            if (recetekodu != "" || hastatc != "")
            {
                tutRecete = recetekodu;
                tutTC = hastatc;
            }
            var degerler = from d in ec.recete where d.recetekodu == tutRecete || d.hastatc == tutTC select d;
            if (!string.IsNullOrEmpty(recetekodu))
            {
                degerler = degerler.Where(x => x.recetekodu.Contains(recetekodu));
            }
            if (!string.IsNullOrEmpty(hastatc))
            {
                degerler = degerler.Where(x => x.hastatc.Contains(hastatc));
            }

            return View(degerler.ToList());
        }
        [Authorize]
        [HttpGet]
        public ActionResult ilacVerOnay(int? receteID)
        {
            stokList sl = new stokList();
            sl.receteListesi = ec.recete.Where(x => x.receteid == receteID).ToList();
            sl.ilacListesi = ec.ilac.ToList();
            var ilaclar = ec.recete.Find(receteID);
            string ilacbarkodlari = ilaclar.ilacbarkod;
            string ilacadeti = ilaclar.ilacadet;
            var barkodDizi = ilacbarkodlari.Split('/');
            var ilacAdetDizi = ilacadeti.Split('/');
            ViewBag.foricin = barkodDizi.Length;
            TempData["BarkodDizi"] = barkodDizi.Length;
            for (int i = 0; i < barkodDizi.Length; i++)
            {
                string ilacbarkod = barkodDizi[i];
                var ilac = ec.ilac.Where(x => x.ilac_barkod == ilacbarkod).FirstOrDefault();
                var recete = ec.recete.Find(receteID);
                TempData["ilacad" + i] = ilac.ilac_ad;
                TempData["ilacadet" + i] = ilacAdetDizi[i].ToString();
                TempData["ilacID" + i] = ilac.ilac_id;
            }
            return View(sl);
        }
        [HttpPost]
        public ActionResult ilacVerOnay(double? ilactutar, int? receteID, string odemeTuru,int? foricin)
        {
            int personelid = Convert.ToInt32(Session["PersonelID"]);
            if (receteID != null && ilactutar != null)
            {
                var recete = ec.recete.Find(receteID);
                recete.ilacVerildiMi = 1;
                ec.SaveChanges();
                string ilacismi = "";
     
                for (int i = 0; i < foricin; i++)
                {
                    int ilacID = Convert.ToInt32(TempData["id" + i]);
                    int stokyok = Convert.ToInt32(TempData["stokyok" + i]);
                    var ilacname = ec.ilac.Find(ilacID);
                    ilacismi +=" "+ilacname.ilac_ad+" - " + TempData["adet" + i].ToString();
                    if (ilacID != stokyok)
                    {
                        int ilacadet = Convert.ToInt32(TempData["adet" + i].ToString());
                        var ilac = ec.ilac.Find(ilacID);
                        ilac.ilacadet -= ilacadet;
                        ec.SaveChanges();
                    }
                }
                fatura ftr = new fatura()
                {
                    hasta_ad = recete.hastaad,
                    hasta_tc = recete.hastatc,
                    fatura_traihi = DateTime.Now,
                    odenen_tutar = Convert.ToDouble(ilactutar),
                    hasta_odeme_turu = odemeTuru,
                    faturayikesenid = personelid,
                    satilanilac=ilacismi
                };
                ec.fatura.Add(ftr);
                ec.SaveChanges();
                return RedirectToAction("ilacVer", "Eczaci");
            }
            else
            {
                return RedirectToAction("ilacVer", "Eczaci");
            }

        }
        [Authorize]
        [HttpGet]
        public ActionResult ilacVerRecetesiz(int? kacAdet)
        {
            stokList sl = new stokList();
            sl.ilac =new SelectList(ec.ilac, "ilac_id", "ilac_ad");
            ViewBag.kacadet = kacAdet;
            return View(sl);
        }
        [Authorize]
        [HttpGet]
        public ActionResult ilacVerRecetesizOnay( int? kacadet,string hastaad,string hastatc, int? ilac1, int? ilac2, int? ilac3, int? ilac4, int? ilac5, int? ilac6, int? ilac7, int? ilacadet1, int? ilacadet2, int? ilacadet3, int? ilacadet4, int? ilacadet5, int? ilacadet6, int? ilacadet7)
        {         
            TempData["hastaad"] = hastaad;
            TempData["hastatc"] = hastatc;
            int sayac = 0;          
            if (ilac1 != null)
            {
         
                var ilac = ec.ilac.Find(ilac1);
                TempData["ilacad" + 1] = ilac.ilac_ad;
                TempData["ilacadet" + 1] = ilacadet1;
                TempData["ilacID" + 1] = ilac.ilac_id;
                sayac++;
            }
            if (ilac2 != null)
            {
                var ilac = ec.ilac.Find(ilac2);
                TempData["ilacad" + 2] = ilac.ilac_ad;
                TempData["ilacadet" + 2] = ilacadet2;
                TempData["ilacID" + 2] = ilac.ilac_id;
                sayac++;
            }
            if (ilac3 != null)
            {
                var ilac = ec.ilac.Find(ilac3);
                TempData["ilacad" + 3] = ilac.ilac_ad;
                TempData["ilacadet" + 3] = ilacadet3;
                TempData["ilacID" + 3] = ilac.ilac_id;
                sayac++;
            }
            if (ilac4 != null)
            {
                var ilac = ec.ilac.Find(ilac4);
                TempData["ilacad" + 4] = ilac.ilac_ad;
                TempData["ilacadet" + 4] = ilacadet4;
                TempData["ilacID" + 4] = ilac.ilac_id;
                sayac++;
            }
            if (ilac5 != null)
            {
                var ilac = ec.ilac.Find(ilac5);
                TempData["ilacad" + 5] = ilac.ilac_ad;
                TempData["ilacadet" + 5] = ilacadet5;
                TempData["ilacID" + 5] = ilac.ilac_id;
                sayac++;
            }
            if (ilac6 != null)
            {
                var ilac = ec.ilac.Find(ilac6);
                TempData["ilacad" + 6] = ilac.ilac_ad;
                TempData["ilacadet" + 6] = ilacadet6;
                TempData["ilacID" + 6] = ilac.ilac_id;
                sayac++;
            }
            if (ilac7 != null)
            {
                var ilac = ec.ilac.Find(ilac7);
                TempData["ilacad" + 7] = ilac.ilac_ad;
                TempData["ilacadet" + 7] = ilacadet7;
                TempData["ilacID" + 7] = ilac.ilac_id;
                sayac++;
            }
            TempData["foricin"] = sayac;
            return View();
           
        }
        [HttpPost]
        public ActionResult ilacVerRecetesizOnay(int? foricin,double? ilactutar, string odemeTuru,string hastaad,string hastatc)
        {
            int personelid = Convert.ToInt32(Session["PersonelID"]);
           
            if (ilactutar != null)
            {   
                string ilacismi = "";
             
                for (int i = 1; i <= foricin; i++)
                {
                    int ilacID = Convert.ToInt32(TempData["id" + i]);
                    int stokyok = Convert.ToInt32(TempData["stokyok" + i]);
           
                    if (ilacID != stokyok)
                    {
                        int ilacadet = Convert.ToInt32(TempData["ilacadeti" + i]);
                        ilacismi +=" "+(TempData["ilacismi" + i].ToString()+" - "+ ilacadet) ;
                        var ilac = ec.ilac.Find(ilacID);
                        ilac.ilacadet -= ilacadet;
                        ec.SaveChanges();
                    }
                }
                fatura ftr = new fatura()
                {
                    hasta_ad = hastaad,
                    hasta_tc = hastatc,
                    fatura_traihi = DateTime.Now,
                    odenen_tutar = Convert.ToDouble(ilactutar),
                    hasta_odeme_turu = odemeTuru,
                    faturayikesenid = personelid,
                    satilanilac = ilacismi
                };
                ec.fatura.Add(ftr);
                ec.SaveChanges();
                return RedirectToAction("Faturalar", "Eczaci");
            }
            else
            {
                return RedirectToAction("ilacVerRecetesiz", "Eczaci");
            }
          
        }
        [Authorize]
        [HttpGet]
        public ActionResult Faturalar(string hastatc)
        {
            int personelid = Convert.ToInt32(Session["PersonelID"]);
            var degerler = from d in ec.fatura where d.faturayikesenid == personelid select d;
            if (!string.IsNullOrEmpty(hastatc))
            {
                degerler = degerler.Where(x => x.hasta_tc.Contains(hastatc));
            }
            var dgr = degerler.OrderBy(x => x.fatura_traihi);
            if (degerler.Count() == 0)
            {
                ViewBag.faturayok="Henüz fatura kesilmemiş";
            }
            return View(dgr.ToList());
        }
        [Authorize]
        [HttpGet]
        public ActionResult FaturaDetay(int? faturaID)
        {
            stokList sl = new stokList();
            sl.faturaListesi = ec.fatura.Where(x => x.fatura_id == faturaID).ToList();
            return View(sl);
        }
    }
}