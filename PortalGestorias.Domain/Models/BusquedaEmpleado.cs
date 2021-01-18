using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Domain.Models
{
   public class BusquedaEmpleado : BusinessEntity
    {
        public virtual Empleado BuscarEmpleado { get; set; }

        public string Login { get; set; }

        [MaxLength(100)]
        public String Email { get; set; }

        public bool Administrador { get; set; }

        [Display(Name = "Modificación")]
        public bool Modificacion { get; set; }

        public bool Consulta { get; set; }

        public String Activos { get; set; }
    }
}

