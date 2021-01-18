using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PortalGestorias.Domain.Entities
{
    public static class EntityExtensions
    {

        public static int? GetId(this BusinessEntity entity)
        {
            if (entity == null) return null;

            return entity.Id;
        }


        public static T ApplyChanges<T>(this T original, T newEntity) where T:BusinessEntity
        {

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                    .Where(p => !(p.PropertyType.IsInterface && p.PropertyType.IsGenericType &&
                                    p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                                    && p.CanWrite).ToList();
            properties.ForEach(p =>
            {
                p.GetValue(original);
                p.SetValue(original, p.GetValue(newEntity));
            });

            return original;
        }
    }
}
