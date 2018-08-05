using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMvc.Models;
using WebMvc.Infrastructure;


namespace WebMvc.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpClient _apiClinet;
        private readonly ILogger<CatalogService> _logger;
        private readonly string _remoteServiceBaseUrl;

        public CatalogService
        (
            IOptionsSnapshot<AppSettings> settings,
            IHttpClient httpClient,
            ILogger<CatalogService> logger
        )
        {
            _settings   = settings;
            _apiClinet  = httpClient;
            _logger     = logger;

            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/catalog/";

            //Console.WriteLine("[CatalogService][Constructor]: - (): " + ());   
            // Console.WriteLine("[CatalogService][Constructor]: - (_settings == null): "      + (_settings == null));            
            // Console.WriteLine("[CatalogService][Constructor]: - (_apiClinet == null): "     + (_apiClinet == null)); 
            // Console.WriteLine("[CatalogService][Constructor]: - (_logger == null): "        + (_logger == null));
            // Console.WriteLine("[CatalogService][Constructor]: - _remoteServiceBaseUrl: "    + _remoteServiceBaseUrl);
        }

        public async Task<Catalog> GetCatalogItems
        (
            int page,
            int take,
            int? brand,
            int? type
        )
        {
            var allCatalogItemsUri = ApiPaths.Catalog.GetAllCatalogItems
            (
                _remoteServiceBaseUrl,
                page,
                take,
                brand,
                type
            );

            //Console.WriteLine("[CatalogService][GetCatalogItems]: - (ApiPaths.Catalog.GetAllCatalogItems == null): " + (allCatalogItemsUri == null));   

            var dataString = await _apiClinet.GetStringAsync
            (
                allCatalogItemsUri
            );

            // Console.WriteLine("[CatalogService][GetCatalogItems]: - (_apiClinet.GetStringAsync == null): "  + (dataString == null));   
            // Console.WriteLine("[CatalogService][GetCatalogItems]: - (_apiClinet.GetStringAsync): \n"        + (dataString) + "\n");   

            var response = JsonConvert.DeserializeObject<Catalog>
            (
                dataString
            );

            // Console.WriteLine("[CatalogService][GetCatalogItems]: - (JsonConvert.DeserializeObject<Catalog>): " + (response));   
            // Console.WriteLine("[CatalogService][GetCatalogItems]: - (JsonConvert.DeserializeObject<Catalog> == null): " + (response == null));   
            // Console.WriteLine("[CatalogService][GetCatalogItems]: - (returning): \n" + (response)  + "\n");   

            return response;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var getBrandsUri    = ApiPaths.Catalog.GetAllBrands(_remoteServiceBaseUrl);
            var dataString      = await _apiClinet.GetStringAsync(getBrandsUri);

            Console.WriteLine("[CatalogService][GetBrands()]: - _remoteServiceBaseUrl: "    + _remoteServiceBaseUrl);       
            Console.WriteLine("[CatalogService][GetBrands()]: - getBrandsUri: "    + getBrandsUri);   
            Console.WriteLine("[CatalogService][GetCatalogItems]: - (_apiClinet.GetStringAsync): \n"        + (dataString) + "\n");            

            var brandItems = new List<SelectListItem>
            {
                new SelectListItem() 
                { 
                    Value       = null,
                    Text        = "All",
                    Selected    = true
                }
            };

            var brands = JArray.Parse(dataString);

            foreach ( var brand in brands.Children<JObject>() )
            {
                brandItems.Add
                (
                    new SelectListItem()
                    {
                        Value = brand.Value<string>("id"),
                        Text = brand.Value<string>("brand")
                    }
                );
            }
            return brandItems;
        }
        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var getTypesUri    = ApiPaths.Catalog.GetAllTypes(_remoteServiceBaseUrl);
            var dataString      = await _apiClinet.GetStringAsync(getTypesUri);

            Console.WriteLine("[CatalogService][GetTypes()]: - _remoteServiceBaseUrl: "    + _remoteServiceBaseUrl);       
            Console.WriteLine("[CatalogService][GetTypes()]: - getTypesUri: "    + getTypesUri);   
            Console.WriteLine("[CatalogService][GetTypes]: - (_apiClinet.GetStringAsync): \n"        + (dataString) + "\n");            

            var typeItems = new List<SelectListItem>
            {
                new SelectListItem() 
                { 
                    Value       = null,
                    Text        = "All",
                    Selected    = true
                }
            };

            var types = JArray.Parse(dataString);

            foreach ( var type in types.Children<JObject>() )
            {
                typeItems.Add
                (
                    new SelectListItem()
                    {
                        Value = type.Value<string>("id"),
                        Text =  type.Value<string>("type")
                    }
                );
            }
            return typeItems;
        }        
    }
}