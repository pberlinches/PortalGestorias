using System.Collections.Generic;

namespace PortalGestorias.Web.Models
{
    public class BreadcrumbItem
    {
        public string Text { get; set; }

        public string Action { get; set; }

        public object Parameters { get; set; }
    }

    public class Breadcrumb
    {
        public Breadcrumb()
        {
            Items = new Queue<BreadcrumbItem>();
        }

        public Queue<BreadcrumbItem> Items { get; set; }
    }
}