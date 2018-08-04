using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Data;
using ProductCatalogApi.Domain;
using ProductCatalogApi.ViewModels;

namespace ProductCatalogApi
{
    [Produces("application/json")]
    [Route("api/Catalog")]
    public class CatalogController : Controller
    {
        private readonly CatalogContext _catalogContext;
        private readonly IOptions<CatalogSettings> _settings;
        
        public CatalogController(CatalogContext ctlgCntx, IOptions<CatalogSettings> stngs)
        {
            _catalogContext = ctlgCntx;
            _settings = stngs;
            ( (DbContext) _catalogContext ).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            // Example
            // string url = _settings.Value.ExternalCatalogBaseUrl;
        }

        [HttpGet]
        [Route("[action]")] // replacment token that is replaced by method name
        public async Task<IActionResult> CatalogTypes()
        {
            var catTypes = await _catalogContext.CatalogTypes.ToListAsync();
            return Ok(catTypes);
        }

        [HttpGet]
        [Route("[action]")] // replacment token that is replaced by method name
        public async Task<IActionResult> CatalogBrands()
        {
            var catBrands = await _catalogContext.CatalogBrands.ToListAsync();
            return Ok(catBrands);
        }        

        // GET api/Catalog/allitems/
        [HttpGet]
        [Route("[action]")] // replacment token that is replaced by method name
        public async Task<IActionResult> AllItems()
        {
            Console.WriteLine("[HttpGet][CatalogController.AllItems]");

            var catalogItems = await _catalogContext.CatalogItems
                                .OrderBy(c => c.Id)
                                .ToListAsync();
            
            Console.WriteLine("[HttpGet][CatalogController.AllItems] catalogItems.Count: " + catalogItems.Count);

            return Ok(catalogItems);
        }


        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if ( id <= 0 )
            {
                return BadRequest();
            }
            var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync( c => c.Id == id);
            if ( item != null)
            {
                item.PictureUrl = item.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced",
                                                            _settings.Value.ExternalCatalogBaseUrl);
                return Ok(item);
            }
            return NotFound();
        }

        // GET api/Catalog/items/[?pageSize=4&pageIndex=3]
        // [HttpGet]
        // [Route("[action]")] // replacment token that is replaced by method name
        // public async Task<IActionResult> Items( [FromQuery] int pageSize    = 6,
        //                                         [FromQuery] int pageIndex   = 0)
        // {
        //     var totalItems = await _catalogContext.CatalogItems.LongCountAsync();
        //     var itemsOnPage = await _catalogContext.CatalogItems
        //                         .OrderBy(c => c.Name)
        //                         .Skip(pageSize * pageIndex)
        //                         .Take(pageSize)
        //                         .ToListAsync();
        //     itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
        //     var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
        //     return Ok(model);
        // }

        // GET api/Catalog/items/[?pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]")] // replacment token that is replaced by method name
        public async Task<IActionResult> Items( [FromQuery] int pageSize    = 6,
                                                [FromQuery] int pageIndex   = 0)
        {
            var totalItems = await _catalogContext.CatalogItems.LongCountAsync();
            var itemsOnPage = await _catalogContext.CatalogItems
                                .OrderBy(c => c.Id)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
            return Ok(model);
        }

        // GET api/Catalog/items/withname/Wonder
        // [HttpGet]
        // [Route("[action]/withname/{name:minlength(1)}")]
        // public async Task<IActionResult> Items( string name)
        // {
        //     var catItemsCount = await _catalogContext.CatalogItems
        //                         .Where(c => c.Name.StartsWith(name))
        //                         .LongCountAsync();
        //     var catalogItems = await _catalogContext.CatalogItems
        //                         .Where(c => c.Name.StartsWith(name))
        //                         .OrderBy(c => c.Id)
        //                         .ToListAsync();
        //     catalogItems = ChangeUrlPlaceHolder(catalogItems);
        //     return Ok(catalogItems);
        // } 

        // GET api/Catalog/items/withname/Wonder?pageSize=2&pageIndex=0
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items( string name,
                                                [FromQuery] int pageSize    = 6,
                                                [FromQuery] int pageIndex   = 0)
        {
            var totalItems = await _catalogContext.CatalogItems
                                .Where(c => c.Name.StartsWith(name))
                                .LongCountAsync();
            var itemsOnPage = await _catalogContext.CatalogItems
                                .Where(c => c.Name.StartsWith(name))
                                .OrderBy(c => c.Name)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
            return Ok(model);
        }        

        // GET api/Catalog/Items/type/1/brand/null
        [HttpGet]
        // [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        // public async Task<IActionResult> Items( int? catalogTypeId,
        //                                         int? catalogBrandId)
        // {
        //     var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

        //     if ( catalogTypeId.HasValue)
        //     {
        //         root = root.Where(c => c.CatalogTypeId == catalogTypeId);
        //     }
        //     if ( catalogBrandId.HasValue)
        //     {
        //         root = root.Where(c => c.CatalogBrandId == catalogBrandId);
        //     }            

        //     var catItemsCount = await root.LongCountAsync();
        //     var catalogItems = await root
        //                         .OrderBy(c => c.Id)
        //                         .ToListAsync();
        //     catalogItems = ChangeUrlPlaceHolder(catalogItems);
        //     return Ok(catalogItems);
        // }            


        // GET api/Catalog/Items/type/1/brand/null[?pageSize=4&pageIndex=0
       [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> Items( int? catalogTypeId,
                                                int? catalogBrandId,
                                                [FromQuery] int pageSize    = 6,
                                                [FromQuery] int pageIndex   = 0)
        {
            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

            if ( catalogTypeId.HasValue)
            {
                root = root.Where(c => c.CatalogTypeId == catalogTypeId);
            }
            if ( catalogBrandId.HasValue)
            {
                root = root.Where(c => c.CatalogBrandId == catalogBrandId);
            }            

            var totalItems = await root.LongCountAsync();
            var itemsOnPage = await root
                                .OrderBy(c => c.Name)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
            return Ok(model);
        }            

        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateProduct( [FromBody] CatalogItem productToCreate )
        {
            Console.WriteLine("[HttpPost][CatalogController.CreateProduct]");

            Console.WriteLine("Passed-in productToCreate: ");
            Console.WriteLine(productToCreate.Description);

            var item = new CatalogItem
            {
                CatalogBrandId  = productToCreate.CatalogBrandId,
                CatalogTypeId   = productToCreate.CatalogTypeId,
                Description     = productToCreate.Description,
                Name            = productToCreate.Name,
                PictureFileName = productToCreate.PictureFileName,
                Price           = productToCreate.Price
            };

            Console.WriteLine("Newly-created catalog item:");
            Console.WriteLine(item.Description);
            
            Console.WriteLine("Adding creatd product to dbContext");
            _catalogContext.CatalogItems.Add(item);

            Console.WriteLine("Saving created product to DB");
            await _catalogContext.SaveChangesAsync();

            Console.WriteLine("Calling GetItemById, passing-in ID of " + item.Id );
            //return CreatedAtAction( nameof(GetItemById), new { id = item.Id } );
            return CreatedAtAction( nameof(GetItemById), new { id = item.Id }, productToCreate ); 
        }

        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateProduct( [FromBody] CatalogItem productToUpdate)
        {
            var catalogItem = await _catalogContext.CatalogItems
                                .SingleOrDefaultAsync( ci => ci.Id == productToUpdate.Id);
            
            if ( catalogItem == null)
            {
                return NotFound( new { Message = $"Catalog item with an ID of {productToUpdate.Id} was not found. Cannot update" } );
            }

            catalogItem = productToUpdate;
            _catalogContext.CatalogItems.Update(catalogItem);
            await _catalogContext.SaveChangesAsync();

            //return CreatedAtAction( nameof(GetItemById), new { id = catalogItem.Id } );
            return CreatedAtAction( nameof(GetItemById), new { id = catalogItem.Id }, catalogItem ); 
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productToDelete = await _catalogContext.CatalogItems
                                    .SingleOrDefaultAsync( ci => ci.Id == id);
            
            if ( productToDelete == null )
            {
                return NotFound( new { Message = $"Catalog item with an ID of {id} was not found. Cannot delete" } );
            }

            _catalogContext.CatalogItems.Remove(productToDelete);
            await _catalogContext.SaveChangesAsync();
            return NoContent();
        }
        
        private List<CatalogItem> ChangeUrlPlaceHolder(List<CatalogItem> items)
        {
            items.ForEach
            (            
                x => x.PictureUrl = x.PictureUrl.Replace
                    (
                        "http://externalcatalogbaseurltobereplaced",
                        _settings.Value.ExternalCatalogBaseUrl
                    )
            );
            return items;  
        }
    }
}