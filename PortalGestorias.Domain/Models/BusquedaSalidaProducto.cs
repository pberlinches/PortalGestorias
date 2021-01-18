using System;
using PortalGestorias.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PortalGestorias.Domain.Models
{
   public class BusquedaSalidaProducto : BusinessEntity
    {
        [Display(Name = "Código de Barras")]
        public String CodigoBarras { get; set; }


        [Display(Name = "Nº de Serie")]
        public String NumeroSerie { get; set; }

        [Editable(true)]
        [ReadOnly(false)]
        [Display(Name = "Código de Barras")]
        public bool CheckCodigoBarras { get; set; }

        [Editable(true)]
        [ReadOnly(false)]
        [Display(Name = "Nº de Serie")]
        public bool CheckNumeroSerie { get; set; }

        public String Destinatario { get; set; }

        [Display(Name = "Estado")]
        public virtual Estado EstadoDevolucion { get; set; }

    }
}
