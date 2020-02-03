using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFSnake.Model;
namespace WPFSnake.Persistence
{
    class GameData
    {
        public List<Point> Snake { get; set; }
        public Tile[,] Map  { get; set; }
        public Direction Way { get; set; }
        public GameData(List<Point> _Snake, Tile[,] _Map, Direction _Way  )
        {
            Snake = new List<Point>();
            foreach (var p in _Snake)
                Snake.Add(p);

            Map = new Tile[_Map.GetLength(0), _Map.GetLength(0)];
            for(int i = 0; i < _Map.GetLength(0); ++i)
            {
                for (int k = 0; k < _Map.GetLength(0); ++k)
                {
                    Map[i, k] = _Map[i, k];
                }
            }

            Way = _Way;
        }
    }
}
