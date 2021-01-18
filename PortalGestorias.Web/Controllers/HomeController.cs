using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using PortalGestorias.Web.Security;
using Microsoft.Owin.Security;
using Thinktecture.IdentityModel.Mvc;
using PortalGestorias.Infrastructure.Data;
using System.Linq;
using System;

namespace PortalGestorias.Web.Controllers
{
    public class HomeController : Controller
    {
        CrmDbContext Db = new CrmDbContext();
        protected int RowsPerPage = 20;

        [ResourceAuthorize(DataOperation.Read, Controllers.Home)]
        public ActionResult Index(int? page)
        {

            var userId = Convert.ToInt32(((ClaimsPrincipal)User).FindFirst("id").Value);
            var fechaLimite = DateTime.Now.AddDays(-15);
            var skip = (page - 1) * RowsPerPage ?? 0;
            var entidades = Db.Empleados.AsQueryable();
            ViewBag.TotalRowCount = entidades.Count();

            entidades = entidades.Where(m => m.Activo == true);
            
            ViewBag.SearchCount = entidades.Count();
            ViewBag.RowsPerPage = 20;
            return View(entidades.OrderBy(m => m.Id).Skip(skip).Take(RowsPerPage).ToList());
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "John Deere - Sistema de Inventario";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.HomeActions.Signin)]
        public ActionResult Signin()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Signout()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = IdSrvConstants.AspNetWebAppUrl
            };

            Request.GetOwinContext().Authentication.SignOut(properties);
            return Redirect("/");
        }

        [AllowAnonymous]
        public void OidcSignOut(string sid)
        {
            var cp = (ClaimsPrincipal)User;
            var sidClaim = cp.FindFirst("sid");

            if (sidClaim != null && sidClaim.Value == sid)
            {
                Request.GetOwinContext().Authentication.SignOut("Cookies");
            }
        }


        





    }
}