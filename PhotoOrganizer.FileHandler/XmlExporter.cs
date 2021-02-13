
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.FileHandler
{
    public static class XmlExporter
    {
        private static IList<Dictionary<string, Tuple<Type, object>>> tableContent;

        public static void ReadTableValues<T>(List<T> table)
        {
            tableContent = new List<Dictionary<string, Tuple<Type, object>>>();
            foreach (var item in table)
            {
                var entityContent = new Dictionary<string, Tuple<Type, object>>();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    var a = item.GetType();
                    var b = a.GetProperty(prop.Name);
                    var c = b.GetValue(item);

                    entityContent.Add(prop.Name, new Tuple<Type, object>(prop.PropertyType, c));
                }
                tableContent.Add(entityContent);
                entityContent = null;
            }
        }
    }
}