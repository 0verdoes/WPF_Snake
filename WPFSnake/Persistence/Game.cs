using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WPFSnake.Model;

namespace WPFSnake.Persistence
{
    class Game
    {
        [Key]
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public ICollection<Field> Fields { get; set; }
        public ICollection<SnakePart> Snake { get; set; }
        public Int32 MapSize { get; set; }
        public Direction Way { get; set; }
        public Game() { 
            Snake = new List<SnakePart>();
            Fields = new List<Field>(); 
        }

    }
}
