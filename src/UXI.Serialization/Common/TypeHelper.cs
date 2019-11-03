using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Common
{
    public static class TypeHelper
    {
        public static bool CanBeNull(Type type) 
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return (type.IsValueType == false) || (Nullable.GetUnderlyingType(type) != null);
        }


        public static bool IsNullable(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsValueType && (Nullable.GetUnderlyingType(type) != null);
        }


        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
