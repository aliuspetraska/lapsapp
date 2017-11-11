using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace LapsWebApi.Controllers
{
    [Route("api/[controller]")]
    public class TracksController : Controller
    {
        private readonly LapsDbContext _lapsDbContext;
        
        public TracksController(LapsDbContext lapsDbContext = null)
        {
            _lapsDbContext = lapsDbContext;
        }
        
        [HttpGet]
        public JsonResult Get()
        {
            if (_lapsDbContext != null)
            {
                return Json(
                    _lapsDbContext.Tracks.OrderByDescending(o => o.Timestamp)
                );
            }
            else
            {
                return Json(new
                {
                    Status = "error",
                    Message = "Can't connect to DB."
                });
            }
        }
    }
}