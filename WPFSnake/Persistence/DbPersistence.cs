using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using WPFSnake.Model;

namespace WPFSnake.Persistence
{
    class DbPersistence //: IPersistence
    {
        SnakeContext _context;

        public DbPersistence(string connection) 
        {
            _context = new SnakeContext(connection);
            _context.Database.CreateIfNotExists();
        }
        
        public List<SaveEntry> List() 
        {
            try
            {
                return _context.Games
                    .OrderByDescending(g => g.Name)
                    .Select(g => new SaveEntry { Name = g.Name, Time = g.Time })
                    .ToList();
            }
            catch ( Exception err)
            {
                throw new DataException("Listázás nem sikerült!");
            }
        }
        public async Task<GameData> LoadAsync(string name) 
        {
            Game game = await _context.Games
                .Include(g => g.Fields)
                .Include(g => g.Snake)
                .SingleAsync(g => g.Name == name);

            List<Point> Snake = new List<Point>();
            Tile[,] Map = new Tile[game.MapSize, game.MapSize];
            Direction Way = game.Way;

            //TODO FINISH FUNCTION
            foreach (var part in game.Snake)
                Snake.Insert(part.Index, new Point(part.X, part.Y));
            
            foreach(var p in game.Fields)
            {
                Map[p.Y, p.X] = p.Type;
            }

            return new GameData(Snake, Map, Way);

        }
        public async Task SaveAsync(string name, GameData data) 
        {
            try
            {
                Game overwriteGame = _context.Games
                        .Include(g => g.Fields)
                        .Include(g => g.Snake)
                        .SingleOrDefault(g => g.Name == name);
                if (overwriteGame != null)
                    _context.Games.Remove(overwriteGame);

                Game DbGame = new Game
                {
                    Name = name,
                    Time = DateTime.Now,
                    Way = data.Way,
                    Snake = new List<SnakePart>(),
                    MapSize = data.Map.GetLength(0)
                };

                for(int i = 0; i < DbGame.MapSize;++i)
                {
                    for (int k = 0; k < DbGame.MapSize; ++k)
                    {
                        Field f = new Field
                        {
                            X = k,
                            Y = i,
                            Type = data.Map[i, k]
                        };
                        DbGame.Fields.Add(f);
                    }
                }

                for(int i = 0;i < data.Snake.Count;++i)
                {
                    DbGame.Snake.Add(
                        new SnakePart
                        {
                            X = data.Snake[i].X,
                            Y = data.Snake[i].Y,
                            Index = i
                        });
                }

                 _context.Games.Add(DbGame);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new DataException("Error during saving!");
            }
        }
        
    }
}
