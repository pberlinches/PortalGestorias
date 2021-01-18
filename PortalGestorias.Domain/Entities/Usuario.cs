using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class Usuario : BusinessEntity
    {

        public bool Administrador { get; set; }

        [Display(Name = "Gestor Proyecto")]
        public bool GestorProyecto { get; set; }

        [Display(Name = "Gestor Empleado")]
        public bool GestorEmpleado { get; set; }

        [Display(Name = "Visor Costes Empleado")]
        public bool VisorCostesEmpleado { get; set; }

        [Display(Name = "Visor Tarifas Empleado")]
        public bool VisorTarifasEmpleado { get; set; }
    }
}
