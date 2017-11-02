using BuyAndSellAppWeb.Models;
using BuyAndSellAppWeb.Repository;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuyAndSellAppWeb.Controllers
{
    public class HomeController : Controller
    {
        //Repo------------------------------------------------------
        private IBuyAndSellRepository _BuyAndSellRepository = null;
        public HomeController()
        {
            this._BuyAndSellRepository = new BuyAndSellRepository();
        }
        public HomeController(IBuyAndSellRepository repository)
        {
            this._BuyAndSellRepository = repository;
        }
        //-----------------------------------------------------------
        [SharePointContextFilter]
        public ActionResult Index()
        {
            MySession.Current.spcontext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (!_BuyAndSellRepository.CheckTermGroupName(context))
                {
                    _BuyAndSellRepository.CreateTermSet(context);
                }
            }
            return RedirectToAction("MainView");
        }
        //MainView---
        public ActionResult MainView()
        {
            List<Advertisment> ViewModel = new List<Advertisment>();

            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (context != null)
                {
                    ViewModel = _BuyAndSellRepository.GetItems(context, _BuyAndSellRepository.ANNONSLISTA);
                    ViewBag.category = _BuyAndSellRepository.GetTaxanomy(context);
                }
            }
            return View(ViewModel);
        }
        //AddAdvertisement---
        public ActionResult AddAdvertisement()
        {

            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (context != null)
                {
                    ViewBag.category = _BuyAndSellRepository.GetTaxanomy(context);
                }
            }
            return View();
        }
        //AddAdvertisementToSharePoint---
        [HttpPost]
        public ActionResult AddAdvertisementToSharePoint(Advertisment formData)
        {
            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (ModelState.IsValid && context != null)
                {
                    _BuyAndSellRepository.AddToSharePoinList(context, formData, _BuyAndSellRepository.ANNONSLISTA);
                }
                else
                {
                    return RedirectToAction("AddAdvertisement");
                }
            }

            return RedirectToAction("MainView");
        }
        //DeleteItem---
        public ActionResult DeleteItem(int? id)
        {
            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (!String.IsNullOrEmpty(id.ToString()) && context != null)
                {
                    _BuyAndSellRepository.DeleteItem(context, _BuyAndSellRepository.ANNONSLISTA, Convert.ToInt32(id));
                }

            }
            return RedirectToAction("MainView");
        }
        //Edit---
        public ActionResult Edit(int? id)
        {
            Advertisment ViewModel = new Advertisment();
            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (context != null)
                {
                    ViewModel = _BuyAndSellRepository.GetListItem(context, _BuyAndSellRepository.ANNONSLISTA, Convert.ToInt32(id));

                    ViewBag.category = _BuyAndSellRepository.GetTaxanomy(context);
                }
            }
            return View(ViewModel);
        }
        //UpdateItemInSharePoint---
        public ActionResult UpdateItemInSharePoint(Advertisment formData)
        {

            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (ModelState.IsValid && context != null)
                {
                    _BuyAndSellRepository.ModifyItem(context, _BuyAndSellRepository.ANNONSLISTA, formData);
                }
                else
                {
                    return RedirectToAction("Edit");
                }
            }
            return RedirectToAction("MainView");
        }
        //Details---
        public ActionResult Details(int? id)
        {
            Advertisment ViewModel = new Advertisment();
            using (var context = MySession.Current.spcontext.CreateUserClientContextForSPHost())
            {
                if (context != null)
                {
                    ViewModel = _BuyAndSellRepository.GetDeatils(context, _BuyAndSellRepository.ANNONSLISTA, Convert.ToInt32(id));
                                        
                }
            }
            return View(ViewModel);
        }

    }
}