using System;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class Ubicacion : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        [Required]
        [MaxLength(100)]
        [Description]
        [Display(Name = "Ubicación")]
        public String Nombre { get; set; }
    }
}
