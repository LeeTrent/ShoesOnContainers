using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMvc.Models;
using WebMvc.Services;
using WebMvc.ViewModels;

namespace WebMvc.Controllers
{
    public class CatalogController : Controller
    {
        private ICatalogService _catalogSvc;

        public CatalogController(ICatalogService catalogSvc) => _catalogSvc = catalogSvc;

        public async Task<IActionResult> Index
        (
            int? BrandFilterApplied,
            int? TypeFilterApplied,
            int? page
        )
        {
            //Console.WriteLine("[CatalogController][Index]: - (): " + ());
            Console.WriteLine("");
            Console.WriteLine( "[CatalogController][Index] - (BrandFilterApplied).: " + (BrandFilterApplied));
            Console.WriteLine( "[CatalogController][Index] - (TypeFileterApplied).: " + (TypeFilterApplied));
            Console.WriteLine( "[CatalogController][Index] - (page)...............: " + (page));

            int itemsPage = 10;
            var catalog = await _catalogSvc.GetCatalogItems
            (
                page ?? 0,
                itemsPage,
                BrandFilterApplied,
                TypeFilterApplied
            );

            foreach (CatalogItem ci in catalog.Data)
            {
                Console.WriteLine("  "); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.CatalogTypeId).: " + (ci.CatalogTypeId)); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.CatalogBrandId): " + (ci.CatalogBrandId)); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.Id)............: " + (ci.Id)); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.Name)..........: " + (ci.Name)); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.Description)...: " + (ci.Description)); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.Price).........: " + (ci.Price)); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.Price).........: " + (ci.CatalogBrandId)); 
                Console.WriteLine("[CatalogController][Index]: (CatalogItem.PictureUrl)....: " + (ci.PictureUrl)); 
            }
            
            // Console.WriteLine("[CatalogController][Index]: (_catalogSvc.GetCatalogItems() == null): " + (catalog == null));
            // Console.WriteLine("[CatalogController][Index]: (_catalogSvc.GetBrands() == null): " + (_catalogSvc.GetBrands() == null));
            // Console.WriteLine("[CatalogController][Index]: (_catalogSvc.GetTypes() == null): " + (_catalogSvc.GetTypes() == null));
            
            var vm = new CatalogIndexViewModel()
            {
                CatalogItems        = catalog.Data,
                Brands              = await _catalogSvc.GetBrands(),
                Types               = await _catalogSvc.GetTypes(),
                BrandFilterApplied  = BrandFilterApplied    ?? 0,
                TypeFilterApplied   = TypeFilterApplied     ?? 0,
                PaginationInfo      = new PaginationInfo
                {
                    ActualPage      = page ?? 0,
                    ItemsPerPage    = itemsPage,
                    TotalItems      = catalog.Count,
                    TotalPages      = (int)Math.Ceiling(((decimal)catalog.Count / itemsPage))
                }
            };

            vm.PaginationInfo.Next  = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1)
                                    ? "is-disabled" : "";
            
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";


            return View(vm);
        }

        // public IActionResult Index()
        // {
        //     return View();
        // }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
