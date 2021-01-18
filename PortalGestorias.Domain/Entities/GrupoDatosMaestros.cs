using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    [ClassName("Dato Maestro")]
    public class GrupoDatosMaestros : BusinessEntity
    {

        [MaxLength(100)]
        [Required]
        public virtual string Nombre { get; set; }

        public virtual string Metadatos { get; set; }

        public virtual ICollection<DatoMaestro> Datos { get; set; }
    }
}