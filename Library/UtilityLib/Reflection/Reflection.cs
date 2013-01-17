using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityLib.Reflection
{
    public static class Reflection
    {
        public static T CreateInstance<T>(object parameters)
        {
            var t = typeof(T);
            return (T)CreateInstance(parameters, t);
        }

        public static object CreateInstance(object parameters, Type targetType)
        {
            object obj;
            if (parameters == null) //means has an default constructor
            {
                obj = targetType.GetConstructor(Type.EmptyTypes).Invoke(null);
            }
            else
            {
                var paramProperties = Property.GetProperties(parameters.GetType());
                var paramTypes = paramProperties.Select(proInfo => proInfo.PropertyType).ToArray();
                var constructor = targetType.GetConstructor(paramTypes);
                obj = constructor.Invoke(paramProperties.Select(propInfo =>
                    Property.GetValue(parameters, propInfo)).ToArray());
            }

            return obj;
        }
    }
}
