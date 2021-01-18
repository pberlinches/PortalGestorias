using System;
using PortalGestorias.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Models
{
   public class EntradaProducto : BusinessEntity
    {
        [Display(Name = "Almacén")]
        public virtual Almacen Almacen { get; set; }

        [Display(Name = "Tipo")]
        public virtual TipoModelo TipoModelo { get; set; }

        public virtual Marca Marca { get; set; }

        public virtual Modelo Modelo { get; set; }

        [Display(Name = "Departamento")]
        public virtual StockRoom StockRoom { get; set; }

        public virtual Ubicacion Ubicacion { get; set; }

        public String CodigoBarras { get; set; }

        [Display(Name = "Nº Pedido")]
        public String NumeroPedido { get; set; }

        [Display(Name = "Nº de Serie")]
        public String NumeroSerie { get; set; }

    }
}
