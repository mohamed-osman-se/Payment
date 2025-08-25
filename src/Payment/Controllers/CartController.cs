using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payment.Models;

namespace Payment.Controllers
{
    [Authorize]
    public class CartController : Controller
    {

        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]

        public async Task<IActionResult> showcart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int id = int.Parse(userId!);

            var orders = await _context.orders
        .Where(o => (o.UserId == id && o.Status!=OrderStatus.Paid))
        .Include(o => o.Product)
        .ToListAsync();

            return View(orders);
        }

        [HttpPost("productId")]

        public async Task<IActionResult> AddToCart(int productId)
        {

            if (!User.Identity!.IsAuthenticated)
                return Redirect("/Account/login");


            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int id = int.Parse(userId!);


            var Order = new Order
            {

                UserId = id,
                ProductId = productId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.orders.Add(Order);
            await _context.SaveChangesAsync();
            return Redirect("/Cart/showcart");
        }
    }
}