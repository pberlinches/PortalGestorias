using System;
using PortalGestorias.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Models
{
   public class BusquedaEstado : BusinessEntity
    {
        public String Nombre { get; set; }
    }
}
