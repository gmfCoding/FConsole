using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastConsole
{
    /// <summary>
    /// Tracks and sets the System.Console foreground and background colours.
    /// </summary>
    class ColourStack
    {
        Stack<ConsoleColor> colors = new Stack<ConsoleColor>();

        /// <summary>
        /// Latest set background colour with in the stack
        /// </summary>
        public ConsoleColor bgl { get; private set; }
        /// <summary>
        /// Latest set foreground colour with in the stack
        /// </summary>
        public ConsoleColor fgl { get; private set; }

        /// <summary>
        /// Saves the foreground and background colours of the console, then sets the current colour to fg and bg if both are supplied.
        /// </summary>
        /// <param name="pushNow"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public ColourStack(bool pushNow = true, ConsoleColor? fg = null, ConsoleColor? bg = null)
        {
            if (pushNow)
            {
                Push();
            }

            if (fg.HasValue || bg.HasValue)
            {
                Set(fg, bg);
            }
        }

        public void Set(ConsoleColor? fg = null, ConsoleColor? bg = null, bool push = false)
        {
            if (fg.HasValue)
            {
                Console.ForegroundColor = fg.Value;
            }

            if (bg.HasValue)
            {
                Console.BackgroundColor = bg.Value;
            }

            if (push)
            {
                Push();
            }
        }

        public void Push()
        {
            fgl = Console.ForegroundColor;
            bgl = Console.BackgroundColor;
            colors.Push(fgl);
            colors.Push(bgl);
        }

        public void Pop()
        {
            if (colors.Count >= 2)
            {
                Console.BackgroundColor = colors.Pop();
                Console.ForegroundColor = colors.Pop();
            }
            else
            {
                colors.Clear();
            }
        }

        public void PopAll()
        {
            // Pop all but the last two
            for (int i = 0; colors.Count > 2; i++)
            {
                colors.Pop();
            }

            // Set the final colour of the console two the last two
            Pop();
        }
    }
}
