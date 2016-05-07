using System;
using System.Collections.Generic;
using System.Linq;

namespace X2CFP
{
    internal class PropertySet
    {
        //Base class for classes that consist of a set of named properties"""

        public Dictionary<string, Properties.IProperty> FieldNames { get; private set; }

        public PropertySet()
        {
            FieldNames = new Dictionary<string, Properties.IProperty>();            
        }

        public string GetProperty(string key)
        {
            string result = FieldNames.FirstOrDefault(p => p.Key == key).Value.Value;
            return result;
        }

        public void AddProperty(Properties.IProperty property)
        {
            if (FieldNames != null) FieldNames[property.Name] = property;
            else throw new Exception("Property Set has not been created correctly");
        }
    }
}
