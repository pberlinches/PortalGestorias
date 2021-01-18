using System;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class TipoModelo : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        [Required]
        [MaxLength(100)]
        [Description]
        [Display(Name = "Tipo")]
        public String Nombre { get; set; }
    }
}
