using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TP5.Core.NetCore.Extensions
{
    public static class ObjectExtensions
    {
        public static TTo Cast<TTo>(this object instance)
        {
            // TODO: implement private, member deep copy

            var result = Activator.CreateInstance(typeof(TTo));

            return instance.MappingTo<TTo>((TTo)result);
        }
        public static TTo Cast<TTo>(this object instance, bool ignoreCase)
        {
            // TODO: implement private, member deep copy

            var result = Activator.CreateInstance(typeof(TTo));

            return instance.MappingTo<TTo>((TTo)result, false, new List<string>().ToArray(), ignoreCase);
        }
        public static TTo Cast<TTo>(this object instance, string[] skipProperties)
        {
            var result = Activator.CreateInstance(typeof(TTo));

            return instance.MappingTo<TTo>((TTo)result, false, skipProperties, false);
        }

        public static TTo MappingTo<TTo>(this object instance, TTo to)
        {
            return instance.MappingTo(to, false, new List<string>().ToArray());
        }

        public static TTo MappingTo<TTo>(this object instance, TTo to, bool skipNull)
        {
            return instance.MappingTo(to, skipNull, new List<string>().ToArray());
        }

        public static TTo MappingTo<TTo>(this object instance, TTo to, string[] skipProperties)
        {
            return instance.MappingTo(to, false, skipProperties);
        }
        public static TTo MappingTo<TTo>(this object instance, TTo to, bool skipNull, string[] skipProperties)
        {
            return instance.MappingTo(to, false, skipProperties, false);
        }

        public static TTo MappingTo<TTo>(this object instance, TTo to, bool skipNull, string[] skipProperties, bool ignoreCase)
        {

            // TODO: implement private, member deep copy
            Dictionary<string, PropertyInfo> toProperties = to.GetType()
                .GetProperties()
                .Where(prop => !skipProperties.Contains(prop.Name))
                .Where(prop => prop.CanWrite)
                .ToDictionary(k => k.Name, v => v);

            var pro = instance.GetType()
                .GetProperties();

            instance.GetType()
                .GetProperties()
                .Where(p => !skipNull || p.GetValue(instance) != null)
                .Where(prop => !skipProperties.Contains(prop.Name))
                .ToList()
                .ForEach(prop =>
                {
                    var sourceName = ignoreCase ? prop.Name.ToLower() : prop.Name;
                    if (toProperties.Values.Any(x =>
                    {
                        var destName = ignoreCase ? x.Name.ToLower() : x.Name;

                        return sourceName == destName && prop.PropertyType == x.PropertyType;
                    }))
                    {
                        toProperties.Values.Where(x => sourceName == (ignoreCase ? x.Name.ToLower() : x.Name))
                            .SingleOrDefault()
                            .SetValue(to, prop.GetValue(instance));
                    }
                });

            return to;
        }

        public static TTo MappingTo_Case<TTo>(this object instance, TTo to)
        {

            // TODO: implement private, member deep copy
            Dictionary<string, PropertyInfo> toProperties = to.GetType()
                .GetProperties()
                //.Where(prop => !skipProperties.Contains(prop.Name))
                .Where(prop => prop.CanWrite)
                .ToDictionary(k => k.Name, v => v);

            //var source = instance.GetType()
            //    .GetProperties()
            //    .ToList();

            instance.GetType().GetInterface("IAgentCaseInf")
                .GetProperties()
                //.Where(p => toProperties.ContainsKey(p.Name))
                //.Where(prop => !skipProperties.Contains(prop.Name))
                .ToList()
                .ForEach(prop =>
                {
                    if (toProperties.ContainsKey(prop.Name)
                        && prop.PropertyType == toProperties[prop.Name].PropertyType)
                    {
                        toProperties[prop.Name].SetValue(to, prop.GetValue(instance));
                    }
                });

            return to;
        }
    }
}
