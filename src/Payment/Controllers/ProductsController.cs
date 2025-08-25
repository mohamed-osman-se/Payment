using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payment.DTOs;
using Payment.Models;

namespace Payment.Controllers
{

    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> list()
        {
            var products = await _context.products.ToListAsync();
            return View(products);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult add() => View();


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> add( ProductDTO productDTO, IFormFile imageFile)
        {

            var uploadPath = Path.Combine(_env.WebRootPath, "ProdctImages");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            var fileUrl = $"{Request.Scheme}://{Request.Host}/ProdctImages/{fileName}";

            var proudct = new Product
            {
                Price = productDTO.Price,
                Name = productDTO.Name,
                Description = productDTO.Description,
                ImageUrl = fileUrl,
            };


            _context.products.Add(proudct);
            await _context.SaveChangesAsync();


            return Redirect("/Products/list");

        }


    }
}