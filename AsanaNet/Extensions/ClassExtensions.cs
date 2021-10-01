using System;

namespace AsanaNet.Extensions
{
    public static class Extensions
    {
        public static object AsSave<T, TSave>(this T item)
            where TSave : class
            where T : class
        {
            var list = item.GetType().GetProperties();
            var inst = Activator.CreateInstance(typeof(TSave));
            foreach (var i in list)
            {
                if (((TSave)inst).GetType().GetProperty(i.Name) == null)
                    continue;
                var valor = i.GetValue(item, null);

                var x = ((TSave)inst).GetType().GetProperty(i.Name);
                if (i.GetType() == x?.GetType())
                {
                    ((TSave)inst).GetType().GetProperty(i.Name)?.SetValue((TSave)inst, valor);
                }
            }
            return (TSave)inst;
        }

        public static object GetPropValue(this object src, string propName)
        {
            return src?.GetType().GetProperty(propName)?.GetValue(src, null);
        }
    }
}