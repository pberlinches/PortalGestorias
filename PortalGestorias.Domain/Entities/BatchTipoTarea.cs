using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class BatchTipoTarea
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Ensamblado { get; set; }

        public string Clase { get; set; }

    }
}
