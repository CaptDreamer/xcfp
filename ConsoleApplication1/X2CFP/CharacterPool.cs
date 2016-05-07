using System.Collections;

namespace X2CFP
{
    internal class CharacterPool
    {
        private readonly string _fname;

        public CharacterPool(string fname)
        {
            _fname = fname;
        }

        public IEnumerable Characters()
        {
            //returns an iterator for the characters in this file

            using (Parser parser = new Parser())
            {
                parser.Load(_fname);
                int count = parser.ReadHeader();

                for(int i = 0; i < count; i++)
                {
                    Character c = new Character();
                    foreach (Properties.IProperty p in parser)
                    {
                        c.AddProperty(p);
                    }
                    yield return c;
                }
            }
        }
    }
}
