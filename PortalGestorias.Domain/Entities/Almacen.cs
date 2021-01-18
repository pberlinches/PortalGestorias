using System;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class Almacen : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Almacén")]
        [Description]
        public String Nombre { get; set; }
    }
}
