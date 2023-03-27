using Microsoft.AspNetCore.Mvc;

namespace Hoist.Controllers
{
    public class ErrorController : Controller
    {

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            try
            {
                switch (statusCode)
                {
                    case 404:
                        return View("Error404");
                    //Add additonal later
                    default:
                        return View("Error");
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
