using System;

namespace VRCAccountGen
{
    class Logger
    {

        public static void Log(object obj)
        {
            string log = obj.ToString().Replace("\a", "");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{DateTime.Now.ToShortTimeString()}] [");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("LunaR");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"] {log}\n");
        }

        public static void LogError(object obj)
        {
            string log = obj.ToString().Replace("\a", "");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{DateTime.Now.ToShortTimeString()}] [");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("LunaR");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{log}\n");
        }
    }
}
