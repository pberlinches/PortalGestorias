using System;
using System.ComponentModel.DataAnnotations;
using PortalGestorias.Domain.Models;
namespace PortalGestorias.Domain.Entities
{
    public class Marca : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        [Required]
        [Description]
        [MaxLength(100)]
        [Display(Name = "Marca")]
        public String Nombre { get; set; }
    }
}
