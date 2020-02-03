using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFSnake.Model;
using System.ComponentModel.DataAnnotations;

namespace WPFSnake.Persistence
{
    class Field
    {
        [Key]
        public Int32 Id  {get; set;}
        public Int32 X   {get; set;}
        public Int32 Y   {get; set;}
        public Tile Type {get; set;}
        public Game Game {get; set;}
    }
}
