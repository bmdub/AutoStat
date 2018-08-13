using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace BW.Diagnostics.StatCollection
{
    static class MathHelpers
    {
        static ConcurrentDictionary<string, Type> _mathHelperTypesByName = new ConcurrentDictionary<string, Type>();

        static MathHelpers()
        {
            var mathHelperTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(type => !type.IsAbstract)
                    .SelectMany(type => type
                        .GetInterfaces()
                        .Where(x =>
                            x.IsGenericType &&
                            x.GetGenericTypeDefinition() == typeof(IMath<>) &&
                            x.ContainsGenericParameters == false) // aka is not NullMath
                        .Select(x => (converteeType: x.GenericTypeArguments[0], converterType: type))))
                .ToList();

            foreach (var converter in mathHelperTypes)
                _mathHelperTypesByName[converter.converteeType.FullName] = converter.converterType;
        }

        public static IMath<T> GetMath<T>()
        {
            if (_mathHelperTypesByName.TryGetValue(typeof(T).FullName, out Type type))
                return Activator.CreateInstance(type) as IMath<T>;
            else
                return null;
        }
    }
}
