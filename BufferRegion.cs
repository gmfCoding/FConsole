using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FastConsole.SystemCalls;

namespace FastConsole
{
    public class BufferRegion
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Coord offset;
        public SmallRect region;

        private CharInfo[] buffer;

        public int layer;

        public BufferRegion(CharInfo[] buffer, short width, short height, SmallRect? region = null, Coord? offset = null)
        {
            this.buffer = buffer;
            this.Width = width;
            this.Height = height;

            this.offset = offset ?? new Coord();
            this.region = region ?? new SmallRect() { Top = 0, Left = 0, Right = width, Bottom = height };
        }

        public CharInfo GetAt(int x, int y)
        {
            if (ValidIndexPos(x,y))
            {
                return buffer[x + y * Width];
            }
            return CharInfo.empty;
        }

        public CharInfo GetAtGlobal(int x, int y)
        {
            if (PointInside(x, y))
            {
                return buffer[(x - offset.X) + (y - offset.Y) * Width];
            }
            return CharInfo.empty;
        }


        //public void Render(SafeFileHandle handle)
        //{
        //    Coord regionTopLeft = new Coord { X = 0, Y = 0 };
        //    Coord regionBottomRight = new Coord { X = (short)region.Right, Y = (short)region.Bottom};
        //    SmallRect rect;

        //    rect.Top = (short)(offset.Y);
        //    rect.Left = (short)(offset.X);

        //    rect.Bottom = (short)(regionBottomRight.Y + offset.Y - 1);
        //    rect.Right = (short)(regionBottomRight.X + offset.X - 1);

        //    bool b = WriteConsoleOutputW(handle, buffer, new Coord() { X = (short)Width, Y = (short)Height }, new Coord() { X = regionTopLeft.X, Y = regionTopLeft.Y }, ref rect);
        //}

        public bool ValidIndexPos(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public bool PointInside(int x, int y)
        {
            return x >= offset.X && y >= offset.Y && x < Width + offset.X && y < Height + offset.Y;
        }
    }
}
