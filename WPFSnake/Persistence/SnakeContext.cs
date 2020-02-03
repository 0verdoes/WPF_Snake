using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace WPFSnake.Persistence
{
    class SnakeContext : DbContext
    {
        public SnakeContext(string connection) : base(connection) { }
        public DbSet<Game> Games { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<SnakePart> Snakes { get; set; }
    }
}
