using System;
using System.Linq;
using System.Threading.Tasks;
using AuthEx.Shared.Security;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace AuthEx.Security.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AuthExUser> _userManager;
        private readonly SignInManager<AuthExUser> _signInManager;
        private readonly IOpenIddictScopeManager _scopeManager;

        public HomeController(
            UserManager<AuthExUser> userManager,
            SignInManager<AuthExUser> signInManager,
            IOpenIddictScopeManager scopeManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _scopeManager = scopeManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("~/connect/authorize")]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (!User.Identity.IsAuthenticated)
                return Challenge();

            // Retrieve the profile of the logged in user.
            var user = await _userManager.GetUserAsync(User) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Set the list of scopes granted to the client application.
            var scopes = request.GetScopes();

            principal.SetScopes(request.GetScopes());
            var resources = _scopeManager.ListResourcesAsync(scopes);
            principal.SetResources(await resources.ToListAsync());

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
