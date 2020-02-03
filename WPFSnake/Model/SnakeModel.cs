using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using WPFSnake.Persistence;
using System.IO;
namespace WPFSnake.Model
{
    enum Tile { ground, wall, apple}
    enum Direction { up, down, left, right}
    struct Point
    {
        public Point(int x, int y) { X = x; Y = y; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public static  bool operator == (Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Point a, Point b) 
        {
            return !(a == b);
        }
        
    }
    class SnakeModel
    {
        public Boolean IsPaused { get; set; } 

        private Timer _timer;
        private Tile[,] Map;
        public Int32 MapSize { get; private set;}
        public List<Point> Snake { get; private set;}
        private Direction dir;
        private Random rand;

        DbPersistence _dataAccess;

        public event EventHandler<GameOverEventArgs> GameOver;
        public event EventHandler SnakeMoved;

        public SnakeModel(DbPersistence dataAcces)
        {
            _dataAccess = dataAcces;

            _timer = new Timer();
            _timer.Interval = 600;

            rand = new Random();
            Snake = new List<Point>();

            _timer.Elapsed += Move;

        }
        

        public Tile GetTile(int x, int y)
        {
            if (x > -1 && y > -1 && x < MapSize && y < MapSize)
                return Map[y, x];
            else
                throw new IndexOutOfRangeException("Watch the mapsize Dummy!");
        }

        public void NewGame(string fname)
        {
            StreamReader file = new StreamReader(fname);
            string line = file.ReadLine();

            MapSize = Int32.Parse(line);

            if (MapSize <= 10)
                throw new FileFormatException("Map width at least 11! Height at least 5");

            Snake.Clear();
            Map = new Tile[MapSize, MapSize];

            for (int i = 0; i < MapSize && !file.EndOfStream; ++i)
            {
                line = file.ReadLine();
                for (int k = 0; k < MapSize; ++k)
                {
                    if (line[k] == '0')
                        Map[i, k] = Tile.ground;
                    else if (line[k] == '1')
                        Map[i, k] = Tile.wall;
                }
            }
            file.Close();

            for (int i = MapSize / 2; i < MapSize / 2 + 5; ++i)
                Snake.Add(new Point(i, MapSize / 2));


            dir = Direction.left;
            GenerateApple();
            _timer.Start();
            IsPaused = false;

        }

        private void GenerateApple()
        {
            int x = rand.Next(1, MapSize - 1),
                y = rand.Next(1, MapSize - 1);
            while ( Snake.Contains(new Point(x,y)) || Map[y, x] == Tile.wall)
            {
                y = rand.Next(1, MapSize - 1);
                x = rand.Next(1, MapSize - 1);
            }
            Map[y, x] = Tile.apple;
        }

        public List<SaveEntry> List()
        {
            return  _dataAccess.List();
        }

        public async Task SaveGameState(string name)
        {
            //generate a gamedata, and save it to _context
            if (_dataAccess == null)
                throw new InvalidOperationException("No data acces provided");
            List<Point> SnakeCopy = new List<Point>();
            foreach (var p in Snake)
                SnakeCopy.Add(p);

            await _dataAccess.SaveAsync(name, new GameData(SnakeCopy, Map, dir));

        }

        public async Task LoadGameState(string name)
        {
            try
            {
                
                IsPaused = true;
                if(_dataAccess == null)
                    throw new InvalidOperationException("No data access is provided.");

                GameData data = await _dataAccess.LoadAsync(name);
                Snake.Clear();
                foreach (var p in data.Snake)
                    Snake.Add(p);

                Map = new Tile[data.Map.GetLength(0), data.Map.GetLength(0)];
                MapSize = data.Map.GetLength(0);
                for (int i = 0; i < data.Map.GetLength(0); ++i)
                    for (int k = 0; k < data.Map.GetLength(0); ++k)
                        Map[i, k] = data.Map[i, k];
                dir = data.Way;

                _timer.Start();
            }
            catch
            {
                throw new FieldAccessException();
            }
        }

        public void Move(object sender, ElapsedEventArgs e)
        {
            if (!IsPaused)
            {
                int dirx = 0,
                    diry = 0;
                //getin matrix directions
                if (dir == Direction.left)
                    dirx = -1;
                else if (dir == Direction.right)
                    dirx = 1;
                else if (dir == Direction.up)
                    diry = -1;
                else if (dir == Direction.down)
                    diry = 1;
                Tile destination = Map[Snake[0].Y + diry, Snake[0].X + dirx];
                // cases
                if (destination == Tile.wall || Snake.Contains(new Point(Snake[0].X + dirx, Snake[0].Y + diry)))
                {
                    IsPaused = true;
                    GameOver(this, new GameOverEventArgs(Snake.Count - 5));
                    return;
                }
                else if (destination == Tile.apple) // Snake grows at the end.. i think it makes sense
                {
                    //Snake.RemoveAt(Snake.Count - 1);
                    Map[Snake[0].Y + diry, Snake[0].X + dirx] = Tile.ground;
                    GenerateApple();
                    Snake.Insert(0, new Point(Snake[0].X + dirx, Snake[0].Y + diry));
                    
                }
                else if(destination == Tile.ground)
                {
                    Snake.RemoveAt(Snake.Count - 1);
                    Snake.Insert(0, new Point(Snake[0].X + dirx, Snake[0].Y + diry));
                    
                }
                
                SnakeMoved(this, EventArgs.Empty);
            }
        }

        public void Turn(Direction to)
        {
            if (IsOpposing(to, dir) || to == dir)
                return;
            else
                dir = to;
        }

        private bool IsOpposing(Direction a, Direction b)
        {
            if (a == Direction.up)
                return b == Direction.down;
            else if (a == Direction.down)
                return b == Direction.up;
            else if (a == Direction.left)
                return b == Direction.right;
            else if (a == Direction.right)
                return b == Direction.left;
            else
                return false;
        }

    }
}
