﻿using System.Threading.Tasks;
using AuthEx.Shared.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthEx.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Unsecured()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secured()
        {
            return View();
        }

        [HttpGet]
        public async Task LogIn()
        {
            if (User.Identity.IsAuthenticated)
                HttpContext.Response.Redirect("/");
            else
                await HttpContext.ChallengeAsync(SecurityConstants.OidcScheme);
        }

        [HttpPost]
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync(SecurityConstants.OidcScheme);
        }
    }
}
