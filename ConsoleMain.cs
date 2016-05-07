using System;
using X2CFP;

public class ConsoleMain
{
    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: X2CFP.exe character_file");
            return 1;
        }
            
        CharacterPool pool = new CharacterPool(args[0]);
        foreach(Character c in pool.Characters())
        {
            Console.WriteLine(c.ToString());
            Console.WriteLine(c.Details());
        }
        return 0;       
    }
}
    