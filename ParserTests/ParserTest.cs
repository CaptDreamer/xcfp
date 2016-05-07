using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using X2CFP;
using X2CFP.Properties;

namespace ParserTests
{
    [TestClass]
    public class TestParserRead
    {
        private byte[] _testdata;

        [TestInitialize]
        public void Initialize()
        {
            byte[] init = { 0x01, 0x02, 0x03, 0x04, 0x05 };
            _testdata = init;
            
        }

        [TestMethod]
        public void TestParserReadFull()
        {
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", _testdata);
                byte[] testassert = parser.Read(5);
                CollectionAssert.AreEqual(_testdata, testassert,
                    string.Format("Read Function did not return correct result: Expected: {0}, Actual: {1}", _testdata,
                        testassert));
            }
        }

        [TestMethod]
        public void TestParserReadPartial()
        {
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", _testdata);
                byte[] testassert1 = parser.Read(4);
                byte[] firstdata = new byte[4];
                Array.Copy(_testdata, 0, firstdata, 0, 4);
                CollectionAssert.AreEqual(firstdata, testassert1,
                    string.Format("Read Function did not return correct result: Expected: {0}, Actual: {1}", firstdata,
                        testassert1));
                byte[] testassert2 = parser.Read(0);
                byte[] empty = {};
                CollectionAssert.AreEqual(empty, testassert2,
                    string.Format("Read Function did not return correct result: Expected: {0}, Actual: {1}", empty,
                        testassert2));
                byte[] testassert3 = parser.Read(1);
                byte[] lastdata = new byte[1];
                Array.Copy(_testdata, 4, lastdata, 0, 1);
                CollectionAssert.AreEqual(lastdata, testassert3,
                    string.Format("Read Function did not return correct result: Expected: {0}, Actual: {1}", lastdata,
                        testassert3));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void TestParserReadBeyondEof()
        {
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", _testdata);
                parser.Read(6);
            }
        }
    }

    [TestClass]
    public class TestParserReadInt
    {
        [TestMethod]
        public void TestReadInt()
        {
            Dictionary<byte[], int> testCases = new Dictionary<byte[], int>
            {
                {new byte[] {0x00,0x00,0x00,0x00}, 0},
                {new byte[] {0x01,0x00,0x00,0x00}, 1},
                {new byte[] {0xff,0xff,0xff,0xff}, -1}
            };
            using (Parser parser = new Parser())
            {
                foreach (KeyValuePair<byte[], int> entry in testCases)
                {

                    parser.Load("Tests", entry.Key);
                    int result = parser.ReadInt();
                    Assert.AreEqual(entry.Value, result,
                        string.Format("Read Int did not match: Expected: {0}, Actual: {1}", entry.Value, result));
                }
            }
        }

        [TestMethod]
        public void TestReadIntMultiple()
        {
            byte[] data = { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", data);
                int result1 = parser.ReadInt();
                Assert.AreEqual(0, result1,
                    string.Format("Read Int did not match: Expected: {0}, Actual: {1}", "0", result1));
                int result2 = parser.ReadInt();
                Assert.AreEqual(1, result2,
                    string.Format("Read Int did not match: Expected: {0}, Actual: {1}", "1", result2));
            }
        }
    }

    [TestClass]
    public class TestParserReadString
    {
        [TestMethod]
        public void TestReadNull()
        {
            byte[] data = { 0x00, 0x00, 0x00, 0x00 };
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", data);
                string result = parser.ReadStr();
                Assert.AreEqual("", result,
                    string.Format("Read String did not match: Expected: {0}, Actual: {1}", "", result));
            }
        }

        [TestMethod]
        public void TestReadString()
        {
            byte[] data = { 0x06, 0x00, 0x00, 0x00, Convert.ToByte('H'), Convert.ToByte('e'), Convert.ToByte('l'), Convert.ToByte('l'), Convert.ToByte('o'), 0x00 };
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", data);
                string result = parser.ReadStr();
                Assert.AreEqual("Hello", result,
                    string.Format("Read String did not match: Expected: {0}, Actual: {1}", "Hello", result));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestReadIncorrectSize()
        {
            byte[] data = { 0x04, 0x00, 0x00, 0x00, Convert.ToByte('H'), Convert.ToByte('e'), Convert.ToByte('l'), Convert.ToByte('l'), Convert.ToByte('o'), 0x00 };
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", data);
                parser.ReadStr();
            }
        }

        [TestMethod]
        public void TestReadStringMultiple()
        {
            byte[] data = { 0x06, 0x00, 0x00, 0x00, Convert.ToByte('H'), Convert.ToByte('e'), Convert.ToByte('l'), Convert.ToByte('l'), Convert.ToByte('o'), 0x00, 0x07, 0x00, 0x00, 0x00, Convert.ToByte('W'), Convert.ToByte('o'), Convert.ToByte('r'), Convert.ToByte('l'), Convert.ToByte('d'), Convert.ToByte('!'), 0x00 };
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", data);
                string result1 = parser.ReadStr();
                Assert.AreEqual("Hello", result1,
                    string.Format("First Part did not match: Expected: {0}, Actual: {1}", "Hello", result1));
                string result2 = parser.ReadStr();
                Assert.AreEqual("World!", result2,
                    string.Format("Second Part did not match: Expected: {0}, Actual: {1}", "World!", result2));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void TestReadStringTruncated()
        {
            byte[] data = { 0x07, 0x00, 0x00, 0x00, Convert.ToByte('H'), Convert.ToByte('e'), Convert.ToByte('l'), Convert.ToByte('l'), Convert.ToByte('o'), 0x00 };
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", data);
                parser.ReadStr();
            }
        }
    }

    [TestClass]
    public class TestParserReadProperty
    {
        private static readonly Encoding Iso = Encoding.GetEncoding("ISO-8859-1");

        private readonly byte[] _nameData1 = Combine(new byte[] { 0x0d, 0x00, 0x00, 0x00 }, Iso.GetBytes("strFirstName".ToCharArray()), new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 });
        private readonly byte[] _typeData = Combine(new byte[] { 0x0c, 0x00, 0x00, 0x00 }, Iso.GetBytes("StrProperty".ToCharArray()), new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 });
        private readonly byte[] _sizeData1 = { 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private readonly byte[] _valueData1 = Combine(new byte[] { 0x04, 0x00, 0x00, 0x00 }, Iso.GetBytes("Ana".ToCharArray()), new byte[] { 0x00 });

        private readonly byte[] _nameData2 = Combine(new byte[] { 0x0c, 0x00, 0x00, 0x00 }, Iso.GetBytes("strLastName".ToCharArray()), new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 });
        private readonly byte[] _sizeData2 = { 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private readonly byte[] _valueData2 = Combine(new byte[] { 0x08, 0x00, 0x00, 0x00 }, Iso.GetBytes("Ramirez".ToCharArray()), new byte[] { 0x00 });


        private static byte[] Combine(byte[] first, byte[] second, byte[] third, byte[] fourth = null)
        {
            int size;
            if (fourth == null) size = first.Length + second.Length + third.Length;
            else size = first.Length + second.Length + third.Length + fourth.Length;
            byte[] ret = new byte[size];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length, third.Length);
            if (fourth != null) Buffer.BlockCopy(fourth, 0, ret, first.Length + second.Length + third.Length, fourth.Length);
            return ret;
        }

        [TestMethod]
        public void TestReadProperty()
        {
            byte[] data1 = Combine(_nameData1, _typeData, _sizeData1, _valueData1);
            using (Parser parser = new Parser())
            {
                parser.Load("Tests", data1);
                IProperty returnedProp = parser.ReadProperty();
                Assert.AreEqual("strFirstName", returnedProp.Name,
                    string.Format("Property Name did not match: Expected: {0}, Actual: {1}", "strFirstName",
                        returnedProp.Name));
                Assert.AreEqual("StrProperty", returnedProp.TypeName,
                    string.Format("Property Type did not match: Expected: {0}, Actual: {1}", "StrProperty",
                        returnedProp.TypeName));
                Assert.AreEqual("Ana", returnedProp.Value,
                    string.Format("Property Value did not match: Expected: {0}, Actual: {1}", "Ana",
                        returnedProp.Value));
            }
        }

        [TestMethod]
        public void TestReadPropertyMultiple()
        {
            byte[] data1 = Combine(_nameData1, _typeData, _sizeData1, _valueData1);
            byte[] data2 = Combine(_nameData2, _typeData, _sizeData2, _valueData2);
            byte[] both = new byte[data1.Length + data2.Length];
            Buffer.BlockCopy(data1, 0, both, 0, data1.Length);
            Buffer.BlockCopy(data2, 0, both, data1.Length, data2.Length);

            using (Parser parser = new Parser())
            {
                parser.Load("Tests", both);
                IProperty returnedProp1 = parser.ReadProperty();
                Assert.AreEqual("strFirstName", returnedProp1.Name,
                    string.Format("Property Name did not match: Expected: {0}, Actual: {1}", "strFirstName",
                        returnedProp1.Name));
                Assert.AreEqual("StrProperty", returnedProp1.TypeName,
                    string.Format("Property Type did not match: Expected: {0}, Actual: {1}", "StrProperty",
                        returnedProp1.TypeName));
                Assert.AreEqual("Ana", returnedProp1.Value,
                    string.Format("Property Value did not match: Expected: {0}, Actual: {1}", "Ana",
                        returnedProp1.Value));
                IProperty returnedProp2 = parser.ReadProperty();
                Assert.AreEqual("strLastName", returnedProp2.Name,
                    string.Format("Property Name did not match: Expected: {0}, Actual: {1}", "strLastName",
                        returnedProp2.Name));
                Assert.AreEqual("StrProperty", returnedProp2.TypeName,
                    string.Format("Property Type did not match: Expected: {0}, Actual: {1}", "StrProperty",
                        returnedProp2.TypeName));
                Assert.AreEqual("Ramirez", returnedProp2.Value,
                    string.Format("Property Value did not match: Expected: {0}, Actual: {1}", "Ramirez",
                        returnedProp2.Value));
            }
        }
    }

    [TestClass]
    public class TestParserReadEmpty
    {
        [TestMethod]
        public void TestReadHeader()
        {
            using (Parser parser = new Parser())
            {
                parser.Load(@"c:\tmp\Empty.bin");
                int header = parser.ReadHeader();
                Assert.AreEqual(header, 0);
            }
        }
    }

    [TestClass]
    public class TestParserReadFile
    {
        [TestMethod]
        public void TestReadHeader()
        {
            using (Parser parser = new Parser())
            {
                parser.Load(@"c:\tmp\Test1.bin");
                int header = parser.ReadHeader();
                Assert.AreEqual(header, 1);
            }
        }
    }
}