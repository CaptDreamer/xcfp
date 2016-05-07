using System.Linq;

namespace X2CFP.Properties
{
    internal sealed class StructProperty : Property
    {
        //"""Struct Property - a struct represented as a sequence of properties ended
        //with 'None'"""

        internal PropertySet PropertySet { get; private set; }

        public StructProperty()
        {
            PropertySet = new PropertySet();
            TypeName = "StructProperty";
            Value = "";
        }
        
        public override string Unpack(byte[] data)
        {
            Parser.Instance.StructDataLoad(data);
            Name = Parser.Instance.ReadStr();
            Parser.Instance.SkipPadding();
            foreach (IProperty p in Parser.Instance)
            {
                PropertySet.AddProperty(p);
            }
            return PropertySet.ToString();
        }

        public override string ToString()
        {
            string result = "";
            result += string.Format("<struct: {0}>\n", TypeName);
            if (PropertySet != null)
                result = PropertySet.FieldNames.Values.Aggregate(result, (current, prop) => current + '\t' + prop.ToString() + '\n');

            return result;
        }
    }
}
