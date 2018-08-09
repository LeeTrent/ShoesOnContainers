using System;

namespace WebMvc.Infrastructure
{
    public class ApiPaths
    {
        public static class Catalog
        {
            public static string GetAllCatalogItems
            (
                string baseUri, 
                int page, 
                int take, 
                int? brand, 
                int? type
            )
            {
                var filterQs = "";
                
                if ( brand.HasValue || type.HasValue)
                {
                    var brandQs = (brand.HasValue) ? brand.Value.ToString() : "null";
                    var typeQs  = (type.HasValue)  ? type.Value.ToString()  : "null";
                    filterQs = $"/type/{typeQs}/brand/{brandQs}";
                }
                
                // Console.WriteLine("[ApiPaths][Catalog][GetAllCatalogItems]: - (returning): " 
                //                     + ($"{baseUri}items{filterQs}?pageIndex={page}&pageSize={take}"));   

                return $"{baseUri}items{filterQs}?pageIndex={page}&pageSize={take}";          
            }

            public static string GetCatalogItems
            (
                string baseUri,
                int id
            )
            {
                return $"{baseUri}/items/{id}";
            }

            public static string GetAllBrands
            (
                string baseUri
            )
            {
                // Console.WriteLine("[ApiPaths][Catalog][GetAllBrands]: - (baseUri): " 
                //                     + (baseUri));                   
                // Console.WriteLine("[ApiPaths][Catalog][GetAllBrands]: - (returning): " 
                //                     + ($"{baseUri}CatalogBrands"));   

                return $"{baseUri}catalogBrands";
            }

            public static string GetAllTypes
            (
                string baseUri
            )
            {
                // Console.WriteLine("[ApiPaths][Catalog][GetAllTypes]: - (baseUri): " 
                //                     + (baseUri));                 
                // Console.WriteLine("[ApiPaths][Catalog][GetAllTypes]: - (returning): " 
                //                     + ($"{baseUri}CatalogTypes"));   

                return $"{baseUri}catalogTypes"; 
            }        
        }
    }
}