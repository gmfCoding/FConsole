using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using static FastConsole.SystemCalls;

namespace FastConsole
{

    public class FastConsoleInstance
    {
        public static Dictionary<string, ConsoleColor> colours = new Dictionary<string, ConsoleColor>();

        Queue<string> inputQueue = new Queue<string>();

        internal CharInfo[] buf;
        public short width;
        public short height;

        private SmallRect rect;
        internal static SafeFileHandle handle { get; private set; }

        public ColourPair defaultColour = new ColourPair(ConsoleColor.White, ConsoleColor.Black);
        public ColourPair defaultReadColour = new ColourPair(ConsoleColor.White, ConsoleColor.Black);

        public ColourPair currentColour;
        public ColourPair readColour;

        public bool keepColour = false;

        public bool autoWrap = true;
        public bool autoFlush = true;
        public bool processColours = false;


        int flushBoundsMinX = -1;
        int flushBoundsMinY = -1;
        int flushBoundsMaxX = -1;
        int flushBoundsMaxY = -1;

        int curX = 0;
        int curY = 0;

        public FastConsoleInstance(short offsetX, short offsetY, short width, short height)
        {

            this.width = width;
            this.height = height;
            if (!handle.IsInvalid)
            {
                buf = new CharInfo[height * width];
                rect = new SmallRect() { Left = offsetX, Top = offsetY, Right = (short)(width + offsetX), Bottom = (short)(height + offsetY) };
            }

            ResetWriteColour();
        }

        static FastConsoleInstance()
        {
            GenerateHandle();
            for (int i = 0; i <= (int)ConsoleColor.White; i++)
            {
                colours.Add($"<b:{((ConsoleColor)i).ToString().ToLower()}>", (ConsoleColor)i);
                colours.Add($"<{((ConsoleColor)i).ToString().ToLower()}>", (ConsoleColor)i);
            }
        }

        public static void GenerateHandle()
        {
            handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        }

        public void WriteLine(string str, ColourPair? colour = null)
        {
            if (processColours)
            {
                WriteLineColoured(str, colour);
            }
            else
            {
                //stop the autoflush so we don't do it every str.length
                bool af = autoFlush;
                autoFlush = false;
                for (int i = 0; i < str.Length; i++)
                {
                    Write(str[i], colour);
                }
                autoFlush = af;

                NewLine();

                TryAutoFlush();
                ClampCursor();
            }
        }

        public void TryAutoFlush()
        {
            if (autoFlush)
            {
                Flush();
            }
        }



        private void WriteColoured(string str, bool skipFlush = false, ColourPair? colour = null)
        {
            colour = colour ?? defaultColour;

            ColourPair found = colour.Value;
            if (keepColour == false)
                ResetWriteColour();

            bool mdMode = false;
            StringBuilder mdStr = new StringBuilder();
            bool wasAutoFlush = autoFlush;
            autoFlush = false;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (ch == '<')
                {
                    mdStr.Append(ch);
                    mdMode = true;
                }
                else if (ch == '>' && mdMode)
                {
                    mdStr.Append(ch);
                    mdMode = false;
                    if (colours.TryGetValue(mdStr.ToString().ToLower(), out ConsoleColor fndCol))
                    {
                        if (mdStr.ToString().Contains("b:"))
                            found.Background = fndCol;
                        else
                            found.Foreground = fndCol;
                        mdStr.Clear();
                    }
                }
                else if (mdMode)
                {
                    mdStr.Append(ch);
                }
                else
                {
                    this.Write(ch, found);
                }
            }
            autoFlush = wasAutoFlush;

            if (!skipFlush)
                TryAutoFlush();
        }

        private void WriteLineColoured(string str = "", ColourPair? colour = null)
        {
            WriteColoured(str, true, colour);
            NewLine();
            TryAutoFlush();
        }

        public void Write(string str, ColourPair? colour = null)
        {
            colour = colour ?? defaultColour;

            if (processColours)
            {
                WriteColoured(str, true, colour);
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    Write(str[i], colour);
                }
            }

            TryAutoFlush();
        }

        public void Write(char character, ColourPair? colour = null)
        {
            if (character == '\r')
            {
                curX = 0;
                ClampCursor();
                return;
            }

            if (character == '\b')
            {
                curX--;
            }

            if (character == '\n')
            {
                NewLine();
                return;
            }

            WriteAt(character, curX, curY, colour);

            curX++;
            if (autoWrap && curX >= width)
            {
                NewLine();
            }
            ClampCursor();
            TryAutoFlush();
        }

        public void WriteAt(char character, int x, int y, ColourPair? colour = null)
        {
            WriteAtExplicit(character, x, y, colour ?? defaultColour);
        }

        public void WriteAtExplicit(char character, int x, int y, ColourPair colour)
        {
            UpdateBounds(x, y);
            buf[x + y * width].Attributes = (short)((ushort)colour.Foreground | (ushort)colour.Background << 4);
            buf[x + y * width].Char.UnicodeChar = (ushort)character;
        }

        public string ReadLine(ColourPair? colour = null)
        {
            colour = colour ?? readColour;
            Console.SetCursorPosition(curX, curY);
            string read = string.Empty;

            if (inputQueue.Count > 0)
            {
                read = inputQueue.Dequeue();
            }
            else
            {
                ColourStack cs = new ColourStack(true, colour.Value.Foreground, colour.Value.Background);
                read = Console.ReadLine();
                cs.PopAll();
            }
            WriteLine(read, colour);
            return read;
        }

        public ConsoleKeyInfo Read(bool print = true)
        {
            ConsoleKeyInfo key;
            Console.SetCursorPosition(curX, curY);
            if (inputQueue.Count > 0 && inputQueue.Peek().Length == 1)
            {
                string str = inputQueue.Dequeue();
                ConsoleKey cKey = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), str.ToUpper());
                key = new ConsoleKeyInfo(str[0], cKey, str == str.ToUpper(), false, false);
            }
            else
            {
                key = Console.ReadKey(!print);
            }

            
            if (char.IsLetterOrDigit(key.KeyChar) && print)
            {
                Write(key.KeyChar);
            }
            return key;
        }

        public void QueueInput(string str)
        {
            inputQueue.Enqueue(str);
        }

        public void Flush()
        {
            if (Console.WindowWidth == width)
            {
                width = (short)Console.WindowWidth;
            }

            if (!handle.IsInvalid)
            {
                short MinX = (short)Math.Max(0, flushBoundsMinX);
                short MaxX = (short)Math.Max(0, flushBoundsMaxX);
                short MinY = (short)Math.Max(0, flushBoundsMinY);
                short MaxY = (short)Math.Max(0, flushBoundsMaxY);


                if (MaxX - MinX == 0 || MaxY - MinY == 0)
                {
                    return;
                }

                rect = new SmallRect() { Top = MinY, Bottom = MaxY, Left = MinX, Right = MaxX };

                Coord regionTopLeft = new Coord { X = MinX, Y = MinY };
                Coord regionBottomRight = new Coord { X = MaxX, Y = MaxY };

                Coord offset = regionTopLeft;

                rect.Top = (short)(offset.Y);
                rect.Left = (short)(offset.X);

                rect.Bottom = (short)(regionBottomRight.Y + offset.Y - 1);
                rect.Right = (short)(regionBottomRight.X + offset.X - 1);

                bool b = WriteConsoleOutputW(handle, buf, new Coord() { X = width, Y = height }, new Coord() { X = regionTopLeft.X, Y = regionTopLeft.Y }, ref rect);
            }

            Console.SetCursorPosition(curX, curY);

            ResetBounds();
        }
        private void UpdateBounds(int x, int y)
        {
            if (x < flushBoundsMinX || flushBoundsMinX == -1)
                flushBoundsMinX = x;
            if (x > flushBoundsMaxX || flushBoundsMaxX == -1)
                flushBoundsMaxX = x + 1;

            if (y < flushBoundsMinY || flushBoundsMinY == -1)
                flushBoundsMinY = y;
            if (y > flushBoundsMaxY || flushBoundsMaxY == -1)
                flushBoundsMaxY = y + 1;
        }

        private void ResetBounds()
        {
            flushBoundsMinX = -1;
            flushBoundsMaxX = -1;
            flushBoundsMinY = -1;
            flushBoundsMaxY = -1;
        }

        public void LockSize()
        {
            IntPtr _handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(_handle, false);

            if (_handle != IntPtr.Zero)
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
        }

        public void NewLine()
        {
            curX = 0;
            curY++;
            ClampCursor();
        }

        internal void ClampCursor()
        {
            if (curX >= width)
            {
                curX = width;
            }

            if (curY >= height)
            {
                curY = height - 1;
            }
        }

        public void FlushBehaviour(bool autoFlush, bool flushNow = true)
        {
            this.autoFlush = autoFlush;
            if (flushNow)
            {
                this.Flush();
            }
        }

        public void SetCursor(int x, int y)
        {
            curX = x;
            curY = y;
            ClampCursor();
        }

        public void SetReadColour(ColourPair? colour = null)
        {
            readColour = colour ?? defaultReadColour;
        }

        public void ResetWriteColour()
        {
            currentColour = defaultColour;
        }

        public void ResetAllColours()
        {
            ResetWriteColour();
            ResetReadColour();
        }

        public void ResetReadColour()
        {
            readColour = defaultReadColour;
        }
    }
}
