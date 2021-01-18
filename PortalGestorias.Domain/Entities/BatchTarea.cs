using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalGestorias.Domain.Entities
{
    public class BatchTarea : BusinessEntity
    {

        public virtual BatchTipoTarea TipoTarea { get; set; }

        [DisplayFormat(DataFormatString = "dd/MM/yyyy")]
        public DateTime? FechaSolicitud { get; set; }

        public short Estado { get; set; }

        public DateTime? FechaComienzo { get; set; }

        public DateTime? FechaFin { get; set; }

        public string ParamIn { get; set; }

        public string ParamOut { get; set; }

       
    }
}
