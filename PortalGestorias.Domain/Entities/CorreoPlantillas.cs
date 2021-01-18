using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class CorreoPlantillas : BusinessEntity
    {
        public string Asunto { get; set; }

        public string Cuerpo { get; set; }

        public string Adjuntos { get; set; }

        public string TipoCorreo { get; set; }
    }
}
