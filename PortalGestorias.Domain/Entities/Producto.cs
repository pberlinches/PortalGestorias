using System;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class Producto : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        public virtual Estado Estado { get; set; }
        
        [Display(Name = "Nº Pedido")]
        [Required]
        public String NumeroPedido { get; set; }

        [Display(Name = "Tipo de Modelo")]
        public String TipoModelo
        {
            get { return $"{Modelo?.TipoModelo?.Nombre}"; }
        }


        public virtual Marca Marca { get; set; }

        public virtual Modelo Modelo { get; set; }

        public virtual Almacen Almacen { get; set; }

        public virtual Ubicacion Ubicacion { get; set; }

        public virtual StockRoom Departamento { get; set; }

        [Display(Name = "StockRoom")]
        public String StockRoom
        {
            get { return $"{Departamento?.Codigo}  HW STOCKROOM"; }
        }

        [Description]
        [Display(Name = "Código de Barras")]
        public String CodigoBarras { get; set; }

        [Display(Name = "Número de Serie")]
        public String NumeroSerie { get; set; }

        [Display(Name = "Descripción")]
        public String Descripcion { get; set; }

        public String Destinatario { get; set; }


        [Display(Name = "Fecha de Alta")]
        public DateTime FechaEntrada { get; set; }

        [Display(Name = "Fecha de Entrega")]
        public DateTime? FechaEntrega { get; set; }

        [Display(Name = "Fecha de Baja")]
        public DateTime? FechaBaja { get; set; }

        [Display(Name = "Usuario Alta")]
        public virtual Empleado UsuarioAlta { get; set; }

        [Display(Name = "Usuario Entrega")]
        public virtual Empleado UsuarioEntrega { get; set; }

        [Display(Name = "Usuario Baja")]
        public virtual Empleado UsuarioBaja { get; set; }

        [EpplusIgnore]
        [Required]
        public int Cantidad { get; set; }

        public String Valor
        {
            get { return $"{Modelo?.Importe}"; }
        }

        [EpplusIgnore]
        public String defaultBarcode { get; set; }

        [EpplusIgnore]
        public String defaultTipoModelo { get; set; }

        [EpplusIgnore]
        public String defaultModelo { get; set; }

        [EpplusIgnore]
        public string defaultMarca { get; set; }

        [EpplusIgnore]
        public bool Seleccionado { get; set; }
    }
}
