using System;
using System.Reflection;

namespace TfsDeployer.TeamFoundation
{
    public static class PropertyAdapter
    {
        public static void CopyProperties(Type sourceType, object source, Type targetType, object target)
        {
            foreach (var sourceProperty in sourceType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance))
            {
                var targetProperty = targetType.GetProperty(sourceProperty.Name);
                if (targetProperty != null)
                {
                    CopyProperty(sourceProperty, source, targetProperty, target);
                }
            }

            foreach (var interfaceType in sourceType.GetInterfaces())
            {
                CopyProperties(interfaceType, source, targetType, target);
            }
        }

        private static void CopyProperty(PropertyInfo sourceProperty, object source, PropertyInfo targetProperty, object target)
        {
            object sourcePropertyValue;
            try
            {
                sourcePropertyValue = sourceProperty.GetValue(source, null);
            }
            catch (Exception)
            {
                // swallow
                return;
            }

            var compatibleProperties = targetProperty.PropertyType == sourceProperty.PropertyType ||
                                       (targetProperty.PropertyType.IsEnum && sourceProperty.PropertyType.IsEnum);
            if (compatibleProperties && targetProperty.CanWrite)
            {
                targetProperty.SetValue(target, sourcePropertyValue, null);
            }
            else if (sourcePropertyValue != null)
            {
                var targetPropertyValue = targetProperty.GetValue(target, null);
                if (targetPropertyValue == null) return;

                CopyProperties(sourceProperty.PropertyType, sourcePropertyValue, targetProperty.PropertyType, targetPropertyValue);
            }
        }

    }
}