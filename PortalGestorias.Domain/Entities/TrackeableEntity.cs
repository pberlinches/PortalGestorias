using System;
using System.ComponentModel.DataAnnotations;


namespace PortalGestorias.Domain.Entities
{
    public class EpplusIgnore : Attribute { }

    public abstract class TrackeableEntity
    {
        [EpplusIgnore]
        public virtual  bool Activo { get; set; } = true;

        [EpplusIgnore]
        public virtual DateTime? FechaMod { get; set; }

        [EpplusIgnore]
        [MaxLength(100)]
        public virtual string UsuMod { get; set; }


    }
}