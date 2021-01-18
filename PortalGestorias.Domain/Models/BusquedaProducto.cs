using System;
using PortalGestorias.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Models
{
   public class BusquedaProducto : BusinessEntity
    {
        [Display(Name = "Almacén")]
        public virtual Almacen Almacen { get; set; }

        [Display(Name = "Tipo de Modelo")]
        public virtual TipoModelo TipoModelo { get; set; }

        public virtual Estado Estado { get; set; }

        [Display(Name = "Departamento")]
        public virtual StockRoom StockRoom { get; set; }

        public virtual Marca Marca { get; set; }

        [Display(Name = "Nº Pedido")]
        public String NumeroPedido { get; set; }

        public virtual Modelo Modelo { get; set; }

        [Display(Name = "Ubicación")]
        public virtual Ubicacion Ubicacion { get; set; }

        [Display(Name = "Código de Barras")]
        public String CodigoBarras { get; set; }

        [Display(Name = "Número de Serie")]
        public String NumeroSerie { get; set; }

        [Display(Name = "Fecha Entrada Desde")]
        public DateTime? FechaEntradaDesde { get; set; }

        [Display(Name = "Fecha Entrada Hasta")]
        public DateTime? FechaEntradaHasta { get; set; }

        public string selectedArticulos { get; set; }
    }
}
