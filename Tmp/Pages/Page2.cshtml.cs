using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Tmp.Pages
{
    [Authorize]
    public class Page2Model : PageModel
    {
        private readonly ILogger _logger;

        public Page2Model(ILogger<Page2Model> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
