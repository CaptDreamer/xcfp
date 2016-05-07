using System;

namespace X2CFP.Properties
{
    internal abstract class Property : IProperty
    {
        public virtual string Name { get; set; }
        public virtual string TypeName { get; set; }
        public virtual string Value { get; set; }

        public abstract string Unpack(byte[] data);

        public override string ToString()
        {

            return string.Format("{0}: {1}", Name, Value);
        }
    }

    public interface IProperty
    {
        string Name { get; set; }
        string TypeName { get; set; }
        string Value { get; }

        string Unpack(byte[] data);
    }
}