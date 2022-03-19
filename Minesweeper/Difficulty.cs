using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    /// <summary>
    /// Get the number of length and the number of mines depends on the difficulty.
    /// </summary>
    public class Difficulty
    {
        public static Tuple<int, int> EASY { get; } = new Tuple<int, int>(9, 10);
        public static Tuple<int, int> MEDIUM { get; } = new Tuple<int, int>(16, 40);
        public static Tuple<int, int> HARD { get; } = new Tuple<int, int>(25, 60);
    }
}
