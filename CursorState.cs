using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastConsole
{
    public struct CursorState
    {
        public int x;
        public int y;
        public CursorState(bool save = false)
        {
            x = 0;
            y = 0;
            if (save)
                Save();
        }

        public void Save()
        {
            x = Console.CursorLeft;
            y = Console.CursorTop;
        }

        public void Load(bool keep = true)
        {
            if (keep)
                Console.SetCursorPosition(x, y);
        }
    }
}
