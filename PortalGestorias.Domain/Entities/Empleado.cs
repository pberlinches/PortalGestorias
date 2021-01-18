using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class Empleado : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public String Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public String Apellidos { get; set; }

        [Required]
        [MaxLength(100)]
        public String Email { get; set; }

        public bool Administrador { get; set; }

        [Display(Name = "Modificación")]
        public bool Modificacion { get; set; }

        public bool Consulta { get; set; }

        [Description]
        [Display(Name = "Nombre")]
        public String NombreCompleto
        {
            get { return $"{Apellidos}, {Nombre}"; }
        }
    }
 
}
