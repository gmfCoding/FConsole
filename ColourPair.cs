using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastConsole
{


    public struct ColourPair
    {
        private ConsoleColor first;
        private ConsoleColor second;

        public ConsoleColor Foreground { get => first; set => first = value; }
        public ConsoleColor Background { get => second; set => second = value; }

        public ConsoleColor First { get => first; set => first = value; }
        public ConsoleColor Second { get => second; set => second = value; }

        public ColourPair(ConsoleColor? fg = ConsoleColor.White, ConsoleColor? bg = ConsoleColor.Black)
        {
            first = fg.Value;
            second = bg.Value;
        }

        public static implicit operator byte(ColourPair d) => (byte)(((byte)d.second << 4) | (byte)d.first);
        public static implicit operator ColourPair(byte b) => new ColourPair((ConsoleColor)(short)(0xF & b),(ConsoleColor)(b >> 4));

        public static implicit operator short(ColourPair d) => (short) ((ushort)d.Foreground | (ushort) d.Background << 4);

        public static implicit operator ConsoleColor(ColourPair d) => d.first;
        public static implicit operator ColourPair(ConsoleColor b) => new ColourPair(b);


        public static ColourPair GetDefault(ColourPair? colour, ColourPair _default)
        {
            return colour ?? _default;
        }

        public override string ToString()
        {
            return $"<{first}><b:{second}>";
        }
    }
}
