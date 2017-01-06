using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.Objects
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, Object> ToPropertyNameValues<T>(this T type) where T : class
        {
            Dictionary<string, Object> keyStore = new Dictionary<string, Object>();

            foreach (var prop in type.GetType().GetProperties())
            {

                if (prop.CanRead && (prop.PropertyType.IsPrimitive || prop.PropertyType.IsValueType))
                    keyStore.Add(prop.Name, prop.GetValue(type, null));
            }

            return keyStore;
        }
    }
}
