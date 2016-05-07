XCom Character File Parser C# Version
==========================

Full credit to gnutrino for writing the original Python Parser.

You can find the original parser here: https://github.com/gnutrino/xcfp

Usage
-----

If you're using this in your own code simply add: 

```C#
using X2CFP;

public class Example
{
    public static int Main(string[] args)
    { 
        CharacterPool pool = new CharacterPool(args[0]);
        foreach(Character c in pool.Characters())
        {
            Console.WriteLine(c.ToString());
            Console.WriteLine(c.Details());
        }      
    }
}    
```

Coming Soon
-----

Graphical Interface

Editing

Exporting to new Bin file
