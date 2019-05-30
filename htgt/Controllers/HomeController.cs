using System.Web.Mvc;

namespace htgt.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToActionPermanent("Index", "EmailSubscription");
        }
    }
}