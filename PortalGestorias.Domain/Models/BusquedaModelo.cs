using System;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Domain.Models
{
   public class BusquedaModelo : BusinessEntity
    {
        public String Nombre { get; set; }

        public virtual Marca Marca { get; set; }

        public virtual TipoModelo TipoModelo { get; set; }

        public decimal? ImporteDesde { get; set; }

        public decimal? ImporteHasta { get; set; }
    }
}
