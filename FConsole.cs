using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace FastConsole
{
    public class FConsole
    {

        public static FastConsoleInstance console { get; private set; }


        public static ColourPair DefaultColour { get => console.defaultColour; set => console.defaultColour = value; }
        public static ColourPair DefaultReadColour { get => console.defaultReadColour; set => console.defaultReadColour = value; }
  
        public static ColourPair CurrentColour { get => console.currentColour; set => console.currentColour = value; }
        public static ColourPair ReadColour { get => console.readColour; set => console.readColour = value; }

        public static bool AutoWrap { get => console.autoWrap; set => console.autoWrap = value; }
        public static bool AutoFlush { get => console.autoFlush; set => console.autoFlush = value; }
        public static bool ProcessColours { get => console.processColours; set => console.processColours = value; }
        public static bool KeepColour { get => console.keepColour; set => console.keepColour = value; }

        public static void SetCursor(int x, int y)
        {
            console.SetCursor(x, y);
        }

        public static void FlushBehaviour(bool autoFlush, bool flushNow = true)
        {
            console.FlushBehaviour(autoFlush, flushNow);
        }

        public static void WriteLine(string str = "", ColourPair? colour = null)
        {
            console.WriteLine(str, colour);
        }

        public static void Write(string str = "", ColourPair? colour = null)
        {
            console.Write(str, colour);
        }

        public static void NewLine()
        {
            console.NewLine();
        }

        public static void Write(char character, ColourPair? colour = null)
        {
            console.Write(character, colour);
        }

        public static void WriteAt(char character, int x, int y, ColourPair? colour = null)
        {
            console.WriteAt(character, x, y, colour);
        }

        public static void Flush()
        {
            console.Flush();
        }

        public static void SetReadColour(ColourPair? colour = null)
        {
            console.SetReadColour(colour);
        }

        public static string ReadLine(ColourPair? colour = null)
        {
            return console.ReadLine(colour);
        }

        public static ConsoleKeyInfo ReadKey(bool print = true)
        {
            return console.Read(print);
        }

        public static void LockSize()
        {
            console.LockSize();
        }

        public static void QueueInput(string str)
        {
            console.QueueInput(str);
        }

        static FConsole()
        {
            console = new FastConsoleInstance(0, 0, (short)Console.WindowWidth, (short)Console.BufferHeight);
        }
    }
}
