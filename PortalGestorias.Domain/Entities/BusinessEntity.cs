
using System.Linq;


namespace PortalGestorias.Domain.Entities
{
    public abstract class BusinessEntity: TrackeableEntity
    {
        [EpplusIgnore]
        public virtual int Id { get; set; }

        [EpplusIgnore]
        public virtual string DefaultValue
        {
            get
            {
                var prop = this.GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(DescriptionAttribute))).FirstOrDefault();

                if (prop == null)
                {
                    return "No se ha determinado el atributo Description en la entidad";
                }

                return prop.GetValue(this).ToString();
            }
        }
    }
}
