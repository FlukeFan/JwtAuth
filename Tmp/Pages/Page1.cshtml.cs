using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Tmp.Pages
{
    public class Page1Model : PageModel
    {
        private readonly ILogger _logger;

        public Page1Model(ILogger<Page1Model> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
