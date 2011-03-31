using System;
using System.Reflection;
using System.Text;

namespace Readify.Useful.TeamFoundation.Common
{
    public static class TracingExtensions
    {
        public static string ToFieldDump(this object target)
        {
            return target.ToFieldDump(0);
        }

        private static string ToFieldDump(this object target, int indent)
        {
            if (indent > 2) return "(...)";

            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo propertyInfo in target.GetType().GetProperties())
            {
                try
                {
                    string propertyName = propertyInfo.Name;
                    object propertyValue = target.GetType().InvokeMember(propertyInfo.Name, BindingFlags.GetProperty, null, target, new object[] { });

                    string indentString = new string(' ', indent * 2);
                    if (propertyValue == null || propertyValue.GetType().IsPrimitive || propertyValue is string)
                    {
                        sb.AppendFormat("{0}{1,-25}: {2}\n", indentString, propertyName, propertyValue == null ? "(null)" : propertyValue.ToString());
                    }
                    else
                    {
                        sb.AppendFormat("{0}{1,-25}:\n{2}", indentString, propertyName, propertyValue.ToFieldDump(indent + 1));
                    }
                }
                catch (Exception) { } // swallow - don't care.
            }

            return sb.ToString();
        }
    }
}
