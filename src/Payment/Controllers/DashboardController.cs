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
using Payment.Models;

namespace Payment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {

        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.orders
                .Include(o => o.User)
                .Where(o => o.Status == OrderStatus.Paid)
                .Include(o=>o.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }


    }
}