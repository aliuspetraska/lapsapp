using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LapsWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private IHostingEnvironment _env;
        private WebClient webClient = new WebClient();

        public ImportController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string[] output = { "OK" };

            return new JsonResult(output);
        }
    }
}
