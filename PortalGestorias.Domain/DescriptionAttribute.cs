using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalGestorias.Domain
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class DescriptionAttribute : Attribute
    {

        public DescriptionAttribute()
        {

        }

    }
    

}
