using System;
using System.Collections;
using System.IO;
using System.Text;
using X2CFP.Properties;

namespace X2CFP
{
    internal class Parser : IDisposable, IEnumerable
    {
        private string _fileName;
        private MemoryStream _data;
        internal Encoding Iso = Encoding.GetEncoding("ISO-8859-1");

        public static Parser Instance {get; private set;}
        static Parser()
        {
            Instance = new Parser();
        }
        public void Load(string fileName, byte[] raw = null)
        {
            Instance._fileName = fileName;
            byte[] file = raw ?? File.ReadAllBytes(fileName);

            Instance._data = new MemoryStream();
            Instance._data.Write(file, 0, file.Length);

            try
            {
                Instance._data.Seek(0, 0);
            }
            catch
            {
                throw new Exception("Seek Error");
            }
        }

        public void StructDataLoad(byte[] data)
        {
            Instance._data.Position -= data.Length;
        }
        
        public void Dispose() 
        {
            
        }

        public IEnumerator GetEnumerator()
        {
            while (true)
            {
                IProperty prop = Instance.ReadProperty();
                if (prop == null)
                {
                    break;
                }
                yield return prop;
            }
        }

        public int ReadHeader()
        {
            //reads the file header and returns the number of characters in the file
            try
            {
                Instance._data.Seek(0, 0);
            }
            catch
            {
                throw new Exception("Header Error");
            }

            int magic = Instance.ReadInt();

            if (magic != -1)
            {
                throw new Exception(string.Format("Incorrect Magic Number on file {0}: {1}", Instance._fileName, magic));
            }

            IProperty prop = Instance.ReadProperty();

            // empty files don"t have a CharacterPool property
            if (prop.Name == "PoolFileName")
            {
                return 0;
            }
            if (prop.Name != "CharacterPool")
            {
                throw new Exception(string.Format("Expected Property CharacterPool:ArrayPropery, got {0}:{1}", prop.Name, prop.TypeName));
            }
            int count = int.Parse(prop.Value);

            //I"m not actually convinced this is used, even if it is we"re best
            //discarding it and recreating from the actual file name on write
            Instance.Expect("PoolFileName", "StrProperty");

            Instance.Expect("None");

            int checkcount = Instance.ReadInt();

            if (count != checkcount)
                throw new Exception(string.Format("Mismatched character counts: {0} != {1}", count, checkcount));

            return count;
        }

        public IProperty Expect(string name, string typename = null)
        {
            //Read a property expecting it to have name 'name' and type 'typename', if it doesn't throw an exception
            IProperty prop = Instance.ReadProperty();

            //Expect a 'None' property
            if ((name == "None") && (typename == null))
            {
                if (prop != null)
                    throw new Exception(string.Format("Expected 'None' Property, got {0}:{1}", prop.Name, prop.TypeName));
                return null;
            }

            try
            {
                if ((prop.Name != name) || (prop.TypeName != typename))
                    throw new Exception(string.Format("Expected Property {0}:{1}, got {2}:{3}", name, typename, prop.Name, prop.TypeName));
            }
            catch
            {
                throw new Exception(string.Format("Expected Property {0}:{1}, got {2}", name, typename, prop));
            }
            return prop;
        }

        public IProperty ReadProperty()
        {
            //Read a single property from the file, returns the relevant subtype of Property
            string name = Instance.ReadStr();
            Instance.SkipPadding();

            if (name == "None")
                return null;

            string typename = Instance.ReadStr();
            Instance.SkipPadding();
            IProperty prop = PropertyType<IProperty>.Create(typename);
            int size = Instance.ReadInt();
            if (size == 0 && typename == "BoolProperty") size = 1;
            Instance.SkipPadding();
            
            byte[] data = Instance.Read(size);

            prop.Name = name;
            prop.TypeName = typename;
            prop.Unpack(data);
            
            return prop;
        }

        public byte[] Read(int size)
        {
            byte[] buf = new byte[size];
            int len = Instance._data.Read(buf, 0, size);
            if (len != size)
            {
                throw new IOException("Read Error");
            }
            return buf;
        }

        public int ReadInt()
        {
            byte[] buf = new byte[4];
            Instance._data.Read(buf, 0, 4);
            return BitConverter.ToInt32(buf, 0);
        }

        public string ReadStr()
        {
            int size = Instance.ReadInt();
            if (size == 0)
                return "";

            byte[] bstr = Instance.Read(size);

            //strings should be null terminated
            if (bstr[bstr.Length - 1] != 0)
            {
                throw new Exception(string.Format("Incorrect String size: {0}", size));
            }

            Array.Resize(ref bstr, bstr.Length - 1);
            string msg = Instance.Iso.GetString(bstr);

            return msg;
        }

        public void SkipPadding()
        {
            int pad = Instance.ReadInt();
            if (pad != 0)
                throw new Exception(string.Format("Expected null padding DWORD, got {0}", pad));
        }
    }
}
