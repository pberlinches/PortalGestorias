using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    [ClassName("Dato Maestro")]
    public class DatoMaestro : TrackeableEntity
    {
        public virtual int IdCode { get; set; }

        [Required]
        public virtual int IdGrupo { get; set; }

        public virtual GrupoDatosMaestros Grupo { get; set; }

        [MaxLength(100)]
        [Required]
        public virtual string Value { get; set; }

        public virtual string Metadata { get; set; }
    }
}