using System;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class Modelo : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        [Required]
        [MaxLength(100)]
        [Description]
        [Display(Name = "Modelo")]
        public String Nombre { get; set; }

        public virtual Marca Marca { get; set; }

        [Display(Name = "Tipo")]
        public virtual TipoModelo TipoModelo { get; set; }

        [Required]
        public decimal Importe { get; set; }

        public String Barcode { get; set; }
    }
}
