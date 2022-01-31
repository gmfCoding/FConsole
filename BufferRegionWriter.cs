using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FastConsole.SystemCalls;

namespace FastConsole
{
    public class BufferRegionWriter
    {
        
        List<BufferRegion> regions = new List<BufferRegion>();
        private Coord offset;

        public void AddRegion(BufferRegion br)
        {
            regions.Add(br);
        }

        public int GetWidth()
        {
            int max = 0;
            foreach (var item in regions)
            {
                int total = item.offset.X + item.Width;
                max = total > max ? total : max;
            }

            return max;
        }

        public int GetHeight()
        {
            int max = 0;
            foreach (var item in regions)
            {
                int total = item.offset.Y + item.Height;
                max = total > max ? total : max;
            }

            return max;
        }

        public void Render()
        {
            int width = GetWidth();
            int height = GetHeight();

            CharInfo[] buffer = new CharInfo[height * width];
            SmallRect rect = new SmallRect();

            rect.Top = (short)(offset.Y);
            rect.Left = (short)(offset.X);

            rect.Bottom = (short)(width + offset.Y - 1);
            rect.Right = (short)(height + offset.X - 1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CharInfo ch = CharInfo.empty;
                    int z = int.MinValue;

                    foreach (var region in regions)
                    {
                        if (region.PointInside(x,y))
                        {
                            if (region.layer > z)
                            {
                                z = region.layer;
                                ch = region.GetAtGlobal(x, y);
                            }
                        }
                    }
                    buffer[x + y * width] = ch;
                }
            }

            WriteConsoleOutputW(FastConsoleInstance.handle, buffer, new Coord() { X = (short)width, Y = (short)height }, new Coord() { X = 0, Y = 0 }, ref rect);
        }
    }
}
