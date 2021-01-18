using System;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{

    public class StockRoom : BusinessEntity
    {
        public override string ToString()
        {
            return this.DefaultValue;
        }

        [Required]
        [MaxLength(100)]
        [Description]
        public String Codigo { get; set; }

        [MaxLength(100)]
        [Display(Name = "Descripción")]
        public String Descripcion { get; set; }

        [Display(Name = "Departamento")]
        public String StockRoomString
        {
            get { return $"{Codigo}  HW STOCKROOM"; }
        }
    }
}
