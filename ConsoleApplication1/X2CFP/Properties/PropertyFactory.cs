using System;
using System.Collections.Generic;
using System.Reflection;

namespace X2CFP.Properties
{
    internal class PropertyType<TPropertyType> 
    {
        private static readonly Dictionary<string, Type> KnownTypes = new Dictionary<string, Type>();

        static PropertyType()
        {
            CreateTypeMap();
        }

        public static TPropertyType Create(string typeName)
        {
            Type type;
            if (KnownTypes.TryGetValue(typeName, out type))
            {
                return (TPropertyType) Activator.CreateInstance(type);
            }

            throw new ArgumentException("No type registered with this name");
        }

        public static void CreateTypeMap()
        {
            Assembly currAssembly = Assembly.GetExecutingAssembly();
            Type baseType = typeof(Property);
            foreach (Type type in currAssembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract ||
                    !type.IsSubclassOf(baseType))
                {
                    continue;
                }
                KnownTypes.Add(type.Name, type);
            }
        }
    }
}