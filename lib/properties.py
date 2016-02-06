import struct

class PropertyError(Exception):
    pass

class PropertyType(type):
    """Meta class for properties that automatically registers Properties and
    allows querying them by name"""

    KnownTypes = {}

    def __new__(cls, name, bases=None, namespace=None, **kwargs):

        # Allows Proprty Types to be retrived by string name as ProprtyType(str)
        if name in cls.KnownTypes:
            return cls.KnownTypes[name]

        #if we called PropertyType with just a string and didn't find it in
        #KnownTypes we need to throw an error
        if bases is None or namespace is None:
            raise TypeError("Unknown PropertyType: {}".format(name))

        result = type(name, bases, namespace)

        #lets us define abstract base classes that don't go into the KnowTypes
        #dict by leaving off the typename property
        if hasattr(result, 'typename'):
            cls.KnownTypes[result.typename] = result

        return result

class Property(metaclass=PropertyType):
    """Base class for properties"""

    def __init__(self, name, value=None):
        self.name = name
        if value is not None:
            self.value = value

    def __str__(self):
        return str(self.value)

    def unpack(self, data):
        self.value = self._unpack(data)
        return self

    @classmethod
    def _unpack(cls, data):
        raise NotImplementedError()

    @classmethod
    def read_data(cls, cfile, size):
        """read the data of size 'size' from character file 'cfile' - moved to Property to
        handle some odd cases where properties don't quite follow standard
        format"""
        return cfile.read(size)

class IntProperty(Property, metaclass=PropertyType):
    """Integer Property - represented as a little endian DWORD"""

    typename = 'IntProperty'

    @classmethod
    def _unpack(cls, data):
        return struct.unpack('<i', data)[0]

class ArrayProperty(IntProperty, metaclass=PropertyType):
    """Array Property - acts as an IntProperty with value of the number of
    elements in the array"""

    typename = 'ArrayProperty'

class BoolProperty(Property, metaclass=PropertyType):
    """Boolean Property - represented by a single byte, 0x00 is False anything
    else is True"""

    typename = 'BoolProperty'

    @classmethod
    def _unpack(cls, data):
        return struct.unpack('?', data)[0]

    #BoolProperty gives incorrect size - should be 1 but shows as 0
    @classmethod
    def read_data(cls, cfile, size):
        if size == 0:
            size = 1
        return super().read_data(cfile, size)

class StrProperty(Property, metaclass=PropertyType):
    """String Property - repersented by a little endian DWORD containing the
    string length followed by a null terminated string - assuming latin-1
    encoding"""

    typename = 'StrProperty'

    @classmethod
    def _unpack(cls, data):
        size = IntProperty._unpack(data[:4])
        if len(data[4:]) != size:
            raise PropertyError("Incorrect String Size in StrProperty: {}".format(size))
        return data[4:-1].decode("latin_1")

class NameProperty(Property, metaclass=PropertyType):
    """Name Property - represented as a StrProperty followed by a DWORD that is
    usually (but not always) 0. As the function of this DWORD is unknown for
    the moment we just represent the value of NameProperty as a (str, int)
    tuple"""

    typename = 'NameProperty'

    @classmethod
    def _unpack(cls, data):
        name = StrProperty._unpack(data[:-4])
        val = IntProperty._unpack(data[-4:])
        return (name, val)

class StructProprty(Property, metaclass=PropertyType):
    """Struct Property - a struct represented as a sequence of properties ended
    with 'None'"""

    typename = 'StructProperty'

    def unpack(self, data):
        from .parser import PropertyParser
        import io
        with io.BytesIO(data) as f:
            parser = PropertyParser(f)
            for prop in parser.properties():
                setattr(self, prop.name, prop)

        return self

    def __str__(self):
        return self.name

    @classmethod
    def read_data(cls, cfile, size):
        cls.structname = cfile.read_str()
        cfile.skip_padding()

        return super().read_data(cfile, size)
