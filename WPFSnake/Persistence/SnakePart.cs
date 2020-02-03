using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WPFSnake.Persistence
{
    class SnakePart
    {
        [Key]
        public Int32 Id { get; set; }
        public Game Game { get; set; }
        public Int32 Index { get; set; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
    }
}
