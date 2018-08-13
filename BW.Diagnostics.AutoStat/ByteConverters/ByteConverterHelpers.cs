using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace BW.Diagnostics.StatCollection
{
    static class ByteConverterHelpers
    {
        static ConcurrentDictionary<string, Type> _byteConverterTypesByName = new ConcurrentDictionary<string, Type>();

        static ByteConverterHelpers()
        {
            Assembly mscorlib = typeof(ByteConverterHelpers).Assembly;
            var byteConverterTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(type => !type.IsAbstract)
                    .SelectMany(type => type
                        .GetInterfaces()
                        .Where(x =>
                            x.IsGenericType &&
                            x.GetGenericTypeDefinition() == typeof(IByteConverter<>) &&
                            x.ContainsGenericParameters == false) // aka is not DefaultByteConverter
                        .Select(x => (converteeType: x.GenericTypeArguments[0], converterType: type))))
                .ToList();

            foreach (var converter in byteConverterTypes)
                _byteConverterTypesByName[converter.converteeType.FullName] = converter.converterType;
        }

        public static IByteConverter<T> GetByteConverter<T>()
        {
            if (_byteConverterTypesByName.TryGetValue(typeof(T).FullName, out Type type))
                return Activator.CreateInstance(type) as IByteConverter<T>;
            else
                return new DefaultByteConverter<T>();
        }
    }
}
