using System;

namespace X2CFP.Properties
{
    internal sealed class BoolProperty : Property
    {
        public BoolProperty()
        {
            Value = "false";
            TypeName = "BoolProperty";
        }

        public override string Unpack(byte[] data)
        {
            bool b = BitConverter.ToBoolean(data, 0);
            Value = b.ToString();
            return b.ToString();
        }
    }

    internal sealed class IntProperty : Property
    {
        public IntProperty()
        {
            Value = "-1";
            TypeName = "IntProperty";
        }

        public override string Unpack(byte[] data)
        {
            int len = BitConverter.ToInt32(data, 0);
            Value = len.ToString();
            return len.ToString();
        }
    }

    internal sealed class ArrayProperty : Property
    {
        public ArrayProperty()
        {
            Value = "-1";
            TypeName = "ArrayProperty";
        }

        public override string Unpack(byte[] data)
        {
            int len = BitConverter.ToInt32(data, 0);
            Value = len.ToString();
            return Value;
        }
    }

    internal sealed class NameProperty : Property
    {
        private string _enderValue = "";

        public NameProperty()
        {
            Value = "";
            TypeName = "NameProperty";
        }

        public override string Unpack(byte[] data)
        {
            byte[] nameData = new byte[data.Length - 4];
            byte[] valData = new byte[4];
            Array.Copy(data, 0, nameData, 0, data.Length - 4);
            Array.Copy(data, data.Length - 4, valData, 0, 4);


            byte[] strlen = new byte[4];
            byte[] strdata = new byte[data.Length - 4];
            Array.Copy(nameData, 0, strlen, 0, 4);
            int len = BitConverter.ToInt32(strlen, 0);
            Array.Copy(nameData, 4, strdata, 0, nameData.Length - 4);
            string name = Parser.Instance.Iso.GetString(strdata, 0, len - 1);

            int len2 = BitConverter.ToInt32(valData, 0);
            _enderValue = len2.ToString();

            Value = name;
            return Value;
        }
    }

    internal sealed class StrProperty : Property
    {
        public StrProperty()
        {
            Value = "";
            TypeName = "StrProperty";
        }

        public override string Unpack(byte[] data)
        {
            byte[] buf = new byte[4];
            byte[] strdata = new byte[data.Length - 4];
            Array.Copy(data, 0, buf, 0, 4);
            int len = BitConverter.ToInt32(buf, 0);
            Array.Copy(data, 4, strdata, 0, data.Length - 4);
            if (len == 0) return Value;
            Value = Parser.Instance.Iso.GetString(strdata, 0, len - 1);
            return Value;
        }
    }
}
