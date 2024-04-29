using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using WebApiii.DTO;
using WebApiii.Models;

namespace WebApiii.Controllers
{
    //localhost:5000/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsContext _context;

   
        public ProductsController(ProductsContext context)
        {
            _context = context;
        }

        //localhost:5000/api/products => GET
       
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
           var products = await _context.Products.Where(i => i.IsActive).Select(p => ProductToDTO(p)).ToListAsync();

            return Ok(products);
        }


        //localhost:5000/api/products/1 => GET
        [Authorize]
        [HttpGet("{id}")]
        
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Products
                .Where(i => i.ProductId == id)
                .Select(p =>  ProductToDTO(p))
                .FirstOrDefaultAsync();

            if (p == null)
            {
                return NotFound();
            }

            return Ok(p);


        }

        //veri ekleme
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            //durum kodu içeren bir cevap 201
            return CreatedAtAction(nameof(GetProduct), new { id = entity.ProductId }, entity);
        }



        //veri güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id , Product entity)
        {
            if(id != entity.ProductId)
            {
                return BadRequest();//400 lü hata. Kullanıcı tarafından yanlış talep gönderildi
            }
            var product = await _context.Products.FirstOrDefaultAsync(i=>i.ProductId==id);

            if(product == null)
            {
                return NotFound();
            }

            product.ProductName= entity.ProductName;
            product.Price = entity.Price;
            product.IsActive=entity.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (Exception )
            {
                return NotFound();
            }

            //200
            return NoContent();
        }


        //veri tabanında veri silme=delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if(id  == null) 
            { 
                return NotFound(); 
            }

            // ıd bilgisi var ise product bilgisini veri tabanından alalım

            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            //product bilgisini bulamıyorsak
            if (product == null)
            {
                return NotFound();
            }

            //eğer product bilgisini bulabiliyorsak product bilgisini context üzerinden silelim

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {
                return NotFound();
            }


            //200
            return NoContent();
        }




        private static ProductDTO ProductToDTO(Product p)
        {
            var entity = new ProductDTO();

            if(p!= null)
            {
                entity.ProductId = p.ProductId;
                entity.ProductName = p.ProductName;
                entity.Price= p.Price;

            }
            return entity;
            
        }


       
    }
    
}
