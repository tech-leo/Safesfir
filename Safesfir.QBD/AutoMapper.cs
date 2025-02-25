using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Safesfir.QBD
{
    public static class AutoMapper
    {
        public static T2 MapBaseToDerived<T1, T2>(T1? source, T2? target)
        {
            // Get all the properties from the base class
            PropertyInfo[] baseProperties = typeof(T1).GetProperties();

            foreach (var baseProperty in baseProperties)
            {
                // Find matching property in the target class (T2)
                PropertyInfo targetProperty = typeof(T2).GetProperty(baseProperty.Name);

                if (targetProperty != null && targetProperty.CanWrite)
                {
                    // Set the value of the property in the derived class
                    targetProperty.SetValue(target, baseProperty.GetValue(source));
                }
            }
            return target;
        }
    }
}
