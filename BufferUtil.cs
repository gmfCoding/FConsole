using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FastConsole.SystemCalls;

namespace FastConsole
{
    public static class BufferUtil
    {

        public static CharInfo[] CreateBuffer(string str, int size, ColourPair color)
        {
            CharInfo[] buffer = new CharInfo[size];
            for (int i = 0; i < size; i++)
            {
                if (i < str.Length)
                {
                    buffer[i] = new CharInfo { Char = new CharUnion() { UnicodeChar = str[i] }, Attributes = (short)((ushort)color.Foreground | (ushort)color.Background << 4) };
                }
                else
                {
                    buffer[i] = new CharInfo { Char = new CharUnion() { UnicodeChar = ' ' }, Attributes = 0 };
                }
            }

            return buffer;
        }
    }
}
