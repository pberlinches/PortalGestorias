using PortalGestorias.Domain;
using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalGestorias.Web
{
    public static class Extensions
    {
        public static string ClassName<T> (this T clase) where T:class
        {
            return typeof(T).HasAttribute<ClassNameAttribute>() ? typeof(T).GetAttribute<ClassNameAttribute>().Name : typeof(T).Name;
        }

    }
}