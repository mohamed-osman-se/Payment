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
using Payment.Services;

namespace Payment.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public IActionResult create()
        {
            if (User.Identity!.IsAuthenticated)
                return Redirect("/Products/list");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> create(UserCreateDTO user)
        {

            var dbUser = new User
            {
                Name = user.Name,
                Password = user.Password,
                Email = user.Email

            };

            _context.users.Add(dbUser);
            await _context.SaveChangesAsync();

            await AuthService.Auth(HttpContext, dbUser);
            return Redirect("/Products/list");

        }

        [HttpGet]
        public IActionResult login()
        {
            if (User.Identity!.IsAuthenticated)
                return Redirect("/Products/list");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> login(UserLoginDTO userDTO)
        {
            var dbUser = await _context.users.SingleOrDefaultAsync(u => u.Email == userDTO.Email);
            if (dbUser == null || dbUser.Password != userDTO.Password)
                return Unauthorized();
            await AuthService.Auth(HttpContext, dbUser);
            return Redirect("/Products/list");
        }
    }
}