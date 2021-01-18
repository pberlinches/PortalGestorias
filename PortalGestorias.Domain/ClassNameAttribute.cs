using System;

namespace PortalGestorias.Domain
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassNameAttribute : Attribute
    {
        public string Name { get; set; }

        public ClassNameAttribute(string name)
        {
            Name = name;
        }
    }
}