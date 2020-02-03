using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSnake.Model
{
    class GameOverEventArgs : EventArgs
    {
        public Int32 Score { get; set; }
        public GameOverEventArgs(int score)
        {
            Score = score;
        }
    }
}
