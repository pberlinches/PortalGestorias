using System.Web.Mvc;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;

namespace PortalGestorias.Web.Controllers
{
    public class GruposDatosController : DataControllerBase<GrupoDatosMaestros>
    {

        public GruposDatosController(CrmDbContext dbContext)
            : base(dbContext)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "DatosMaestros");
        }
    }
}
