using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace shippAPI.Controllers
{
    [ApiController]
    [Route("/V1/stores/")]
    public class HomeController : ControllerBase
    {
        // GET: /<controller>/
        [HttpGet]
        public ActionResult<string> Get()
        {
            var objStore = new ClassStores();
            var lstStores = new List<ClassStores>();
            
            if (objStore.ListStores(ref lstStores, Convert.ToDouble(HttpContext.Items["latitude"].ToString()), Convert.ToDouble(HttpContext.Items["longitude"].ToString())))
            {
                HttpContext.Items["countStores"] = lstStores.Count();
                return Ok(lstStores.OrderBy(c => c.rawDistance));
            }
            else
            {
                //Error
                return StatusCode(StatusCodes.Status500InternalServerError, new ClassError(true, "Could not get data from database"));
            }
        }
    }
}
